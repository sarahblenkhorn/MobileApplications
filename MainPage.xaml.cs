using Microsoft.Maui.Controls;
using MobileApplicationDev.Models;
using MobileApplicationDev.Services;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

#if ANDROID
using Android;
using Android.Content.PM;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using Microsoft.Maui.ApplicationModel;
#endif


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
            await LoadTermsAsync();

#if ANDROID
            // Request POST_NOTIFICATIONS permission at runtime if needed
            if (ContextCompat.CheckSelfPermission(Android.App.Application.Context, Manifest.Permission.PostNotifications) != Permission.Granted)
            {
                var activity = Platform.CurrentActivity;
                ActivityCompat.RequestPermissions(activity, new[] { Manifest.Permission.PostNotifications }, 0);
            }

            var allTerms = await _db.GetTermsAsync();

            foreach (var term in allTerms)
            {
                var courses = await _db.GetCoursesForTermAsync(term.Id);

                foreach (var course in courses)
                {
                    var now = DateTime.Now;

                    if (course.StartDate.Date == now.Date)
                    {
                        var builder = new AndroidX.Core.App.NotificationCompat.Builder(Android.App.Application.Context, "default")
                            .SetContentTitle($"Course Starting: {course.CourseTitle}")
                            .SetContentText($"Your course '{course.CourseTitle}' starts today!")
                            .SetSmallIcon(Android.Resource.Drawable.IcDialogInfo) // ✅ Uses built-in icon
                            .SetPriority((int)NotificationCompat.PriorityHigh);


                        AndroidX.Core.App.NotificationManagerCompat
                            .From(Android.App.Application.Context)
                            .Notify(course.Id + 1000, builder.Build());
                    }

                    if (course.EndDate.Date == now.Date)
                    {
                        var builder = new AndroidX.Core.App.NotificationCompat.Builder(Android.App.Application.Context, "default")
                            .SetContentTitle($"Course Ending: {course.CourseTitle}")
                            .SetContentText($"Your course '{course.CourseTitle}' ends today!")
                            .SetSmallIcon(Android.Resource.Drawable.IcDialogInfo) // ✅ Uses built-in icon
                            .SetPriority((int)NotificationCompat.PriorityHigh);


                        AndroidX.Core.App.NotificationManagerCompat
                            .From(Android.App.Application.Context)
                            .Notify(course.Id + 2000, builder.Build());
                    }
                }
            }
#endif
        }


        private async Task LoadTermsAsync()
        {
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
            var addTermPage = new AddTermPage(_db);
            await Navigation.PushAsync(addTermPage);
        }

        private async void OnUpdateTermTapped(object sender, EventArgs e)
        {
            if (sender is Label label && label.BindingContext is TermDisplay termDisplay)
            {
                await Navigation.PushAsync(new UpdateTermPage(termDisplay.TermObject, _db));
            }
        }

        private async void OnCourseTapped(object sender, EventArgs e)
        {
            if (sender is VisualElement view && view.BindingContext is Course course)
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

        private async Task SeedEvaluationData(DatabaseService db)
        {
            // Create a term
            var term = new Term
            {
                Title = "Evaluation Term",
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddMonths(6)
            };
            await db.SaveTermAsync(term);

            // Create a course
            var course = new Course
            {
                CourseTitle = "Evaluation Course",
                CourseStatus = "In Progress",
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddMonths(3),
                InstructorName = "Anika Patel",
                InstructorPhone = "555-123-4567",
                InstructorEmail = "anika.patel@strimeuniversity.edu",
                Notes = "Evaluation notes here",
                NotifyOnStart = false,
                NotifyOnEnd = false,
                TermId = term.Id
            };
            await db.SaveCourseAsync(course);

            // Add performance assessment
            var performanceAssessment = new Assessment
            {
                CourseId = course.Id,
                Title = "Performance Evaluation",
                StartDate = DateTime.Today.AddDays(30),
                EndDate = DateTime.Today.AddDays(30),
                Type = "Performance"
            };
            await db.SaveAssessmentAsync(performanceAssessment);

            // Add objective assessment
            var objectiveAssessment = new Assessment
            {
                CourseId = course.Id,
                Title = "Objective Evaluation",
                StartDate = DateTime.Today.AddDays(60),
                EndDate = DateTime.Today.AddDays(60),
                Type = "Objective"
            };
            await db.SaveAssessmentAsync(objectiveAssessment);

            await Application.Current.MainPage.DisplayAlert("Seed Complete", "Evaluation data created.", "OK");
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
                    var assessments = await _db.GetAssessmentsForCourseAsync(course.Id);
                    foreach (var assessment in assessments)
                    {
                        await _db.DeleteAssessmentAsync(assessment);
                    }

                    await _db.DeleteCourseAsync(course);
                }

                await _db.DeleteTermAsync(termDisplay.TermObject);
                await LoadTermsAsync();
            }
        }
    }
}
