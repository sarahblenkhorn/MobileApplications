using Microsoft.Maui.Controls;
using MobileApplicationDev.Models;
using MobileApplicationDev.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MobileApplicationDev
{
    public partial class MainPage : ContentPage
    {
        private readonly DatabaseService _db;

        public ObservableCollection<TermDisplay> Terms { get; set; } = new();

        public MainPage(DatabaseService db)
        {
            InitializeComponent();
            _db = db;
            BindingContext = this;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            Terms.Clear();

            var allTerms = await _db.GetTermsAsync();

            foreach (var term in allTerms)
            {
                var courses = await _db.GetCoursesForTermAsync(term.Id);

                Terms.Add(new TermDisplay
                {
                    Title = term.Title,
                    StartDateText = term.StartDate.ToString("MMM dd"),
                    EndDateText = term.EndDate.ToString("MMM dd"),
                    Courses = courses,
                    TermObject = term
                });
            }
        }

        private async void OnAddButtonClicked(object sender, EventArgs e)
        {
            var addTermPage = App.Services.GetService<AddTermPage>();
            await Navigation.PushAsync(addTermPage);
        }

        private async void OnUpdateTermTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new UpdateTermPage());
        }

        private async void OnCourseTapped(object sender, EventArgs e)
        {
            if (sender is VisualElement view && view.BindingContext is Course course)
            {
                var courseDetailPage = new CourseDetailPage
                {
                    BindingContext = course
                };

                await Navigation.PushAsync(courseDetailPage);
            }
        }

        private async void OnDeleteTermTapped(object sender, EventArgs e)
        {
            if (sender is Label label && label.BindingContext is TermDisplay termDisplay)
            {
                bool confirm = await DisplayAlert("Delete Term", $"Are you sure you want to delete \"{termDisplay.Title}\"?", "Yes", "Cancel");
                if (!confirm) return;

                var courses = await _db.GetCoursesForTermAsync(termDisplay.TermObject.Id);
                foreach (var course in courses)
                {
                    await _db.DeleteCourseAsync(course);
                }

                await _db.DeleteTermAsync(termDisplay.TermObject);
                OnAppearing(); // refresh UI
            }
        }

    }
}








