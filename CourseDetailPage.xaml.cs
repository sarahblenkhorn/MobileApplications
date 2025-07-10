using System;
using System.Linq;
using Microsoft.Maui.Controls;
using MobileApplicationDev.Models;
using MobileApplicationDev.Services;
using Microsoft.Maui.ApplicationModel.DataTransfer;

namespace MobileApplicationDev
{
    public partial class CourseDetailPage : ContentPage
    {
        private readonly DatabaseService _db;

        public CourseDetailPage(DatabaseService db)
        {
            InitializeComponent();
            _db = db;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext is Course course)
            {
                // Re-fetch latest course data from DB
                var updatedCourse = (await _db.GetCoursesForTermAsync(course.TermId))
                                    .FirstOrDefault(c => c.Id == course.Id);

                if (updatedCourse != null)
                {
                    var assessments = await _db.GetAssessmentsForCourseAsync(updatedCourse.Id);
                    updatedCourse.Assessments = assessments;

                    // Re-bind the updated course to refresh the UI
                    BindingContext = updatedCourse;
                }
            }
        }

        private async void OnShareNotesClicked(object sender, EventArgs e)
        {
            if (BindingContext is Course course && !string.IsNullOrWhiteSpace(course.Notes))
            {
                await Share.Default.RequestAsync(new ShareTextRequest
                {
                    Title = $"Share Notes for {course.CourseTitle}",
                    Text = course.Notes
                });
            }
            else
            {
                await DisplayAlert("No Notes", "There are no notes to share for this course.", "OK");
            }
        }

        private async void OnUpdateCourseClicked(object sender, EventArgs e)
        {
            if (BindingContext is Course course)
            {
                var assessments = await _db.GetAssessmentsForCourseAsync(course.Id);
                var performanceAssessment = assessments.FirstOrDefault(a => a.Type == "Performance");
                var objectiveAssessment = assessments.FirstOrDefault(a => a.Type == "Objective");

                await Navigation.PushAsync(new UpdateCoursePage(course, _db, performanceAssessment, objectiveAssessment));
            }
        }
    }
}


