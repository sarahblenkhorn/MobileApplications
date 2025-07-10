using Microsoft.Maui.Controls;
using MobileApplicationDev.Models;
using MobileApplicationDev.Services;
using Plugin.LocalNotification;
using System;
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
            await LoadTermsAsync();

#if ANDROID || IOS
            var allTerms = await _db.GetTermsAsync();

            foreach (var term in allTerms)
            {
                var courses = await _db.GetCoursesForTermAsync(term.Id);

                foreach (var course in courses)
                {
                    // Simulate immediate trigger for testing
                    var now = DateTime.Now;

                    if (course.StartDate.Date == now.Date)
                    {
                        var startNotification = new NotificationRequest
                        {
                            NotificationId = course.Id + 1000, // Unique ID
                            Title = $"Course Starting: {course.CourseTitle}",
                            Description = $"Your course '{course.CourseTitle}' starts today.",
                            Schedule = new NotificationRequestSchedule
                            {
                                NotifyTime = now.AddSeconds(3) // For demo: pop up 3 sec later
                            }
                        };
                        Plugin.LocalNotification.LocalNotificationCenter.Current.Show(startNotification);
                    }

                    if (course.EndDate.Date == now.Date)
                    {
                        var endNotification = new NotificationRequest
                        {
                            NotificationId = course.Id + 2000, // Unique ID
                            Title = $"Course Ending: {course.CourseTitle}",
                            Description = $"Your course '{course.CourseTitle}' ends today.",
                            Schedule = new NotificationRequestSchedule
                            {
                                NotifyTime = now.AddSeconds(5) // For demo: pop up 5 sec later
                            }
                        };
                        Plugin.LocalNotification.LocalNotificationCenter.Current.Show(endNotification);
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
