using System;
using Microsoft.Maui.Controls;
using MobileApplicationDev.Models;
using MobileApplicationDev.Services;

namespace MobileApplicationDev
{
    public partial class UpdateTermPage : ContentPage
    {
        private readonly Term _term;
        private readonly DatabaseService _db;

        public UpdateTermPage(Term term, DatabaseService db)
        {
            InitializeComponent();
            _term = term;
            _db = db;

            termTitleEntry.Text = _term.Title;
            termStartDate.Date = _term.StartDate;
            termEndDate.Date = _term.EndDate;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadCoursesAsync();
        }

        private async Task LoadCoursesAsync()
        {
            var courses = await _db.GetCoursesForTermAsync(_term.Id);
            courseListView.ItemsSource = courses;
        }

        private async void OnAddCourseClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddCoursePage(_term, _db));
        }

        private async void OnCourseTapped(object sender, EventArgs e)
        {
            if (sender is Label label && label.BindingContext is Course course)
            {
                var assessments = await _db.GetAssessmentsForCourseAsync(course.Id);
                course.Assessments = assessments;

                var courseDetailPage = new CourseDetailPage(_db)
                {
                    BindingContext = course
                };

                await Navigation.PushAsync(courseDetailPage);
            }
        }


        private async void OnSaveClicked(object sender, EventArgs e)
        {
            _term.Title = termTitleEntry.Text?.Trim() ?? "Untitled Term";
            _term.StartDate = termStartDate.Date;
            _term.EndDate = termEndDate.Date;

            await _db.SaveTermAsync(_term);
            await DisplayAlert("Saved", "Term updated successfully.", "OK");
            await Navigation.PopAsync();
        }

        private async void OnDeleteCourseClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is Course courseToDelete)
            {
                bool confirm = await DisplayAlert("Delete Course", $"Delete \"{courseToDelete.CourseTitle}\"?", "Yes", "Cancel");
                if (!confirm) return;

                // Delete assessments first (if needed)
                var assessments = await _db.GetAssessmentsForCourseAsync(courseToDelete.Id);
                foreach (var assessment in assessments)
                {
                    await _db.DeleteAssessmentAsync(assessment);
                }

                await _db.DeleteCourseAsync(courseToDelete);

                // Refresh the list
                var refreshedCourses = await _db.GetCoursesForTermAsync(_term.Id);
                courseListView.ItemsSource = refreshedCourses;
            }
        }


        private async void OnCancelClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Cancel", "Discard changes?", "Yes", "No");
            if (confirm)
            {
                await Navigation.PopAsync();
            }
        }
    }
}
