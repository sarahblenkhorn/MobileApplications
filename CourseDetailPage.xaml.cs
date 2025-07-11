using System;
using System.Linq;
using Microsoft.Maui.Controls;
using MobileApplicationDev.Models;
using MobileApplicationDev.Services;
using Microsoft.Maui.ApplicationModel.DataTransfer;

#if ANDROID
using Android;
using Android.Content.PM;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using Microsoft.Maui.ApplicationModel;
#endif


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

#if ANDROID
            // Request POST_NOTIFICATIONS permission if needed
            if (ContextCompat.CheckSelfPermission(Android.App.Application.Context, Manifest.Permission.PostNotifications) != Permission.Granted)
            {
                var activity = Platform.CurrentActivity;
                ActivityCompat.RequestPermissions(activity, new[] { Manifest.Permission.PostNotifications }, 0);
            }

            var now = DateTime.Now;

            foreach (var assessment in updatedCourse.Assessments)
            {
                if (assessment.StartDate.Date == now.Date)
                {
                    var builder = new NotificationCompat.Builder(Android.App.Application.Context, "default")
                        .SetContentTitle($"Assessment Due: {assessment.Title}")
                        .SetContentText($"Your {assessment.Type} assessment is due today!")
                        .SetSmallIcon(Android.Resource.Drawable.IcDialogInfo)
                        .SetPriority(NotificationCompat.PriorityHigh);

                    NotificationManagerCompat
                        .From(Android.App.Application.Context)
                        .Notify(assessment.Id + 3000, builder.Build());
                }
            }
#endif
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


