using Microsoft.Maui.Controls;
using MobileApplicationDev.Models;
using MobileApplicationDev.Services;
using Plugin.LocalNotification;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MobileApplicationDev
{
    public partial class AddCoursePage : ContentPage
    {
        private readonly Term _term;
        private readonly DatabaseService _db;

        public AddCoursePage(Term term, DatabaseService db)
        {
            InitializeComponent();
            _term = term;
            _db = db;

            statusPicker.ItemsSource = new List<string> { "In Progress", "Completed", "Dropped", "Plan to Take" };
        }

        private async Task ScheduleAssessmentNotification(Assessment assessment)
        {
            if (!assessmentNotifySwitch.IsToggled) return;

            await LocalNotificationCenter.Current.Show(new NotificationRequest
            {
                NotificationId = assessment.Id + 3000,
                Title = $"{assessment.Type} Assessment Due",
                Description = $"{assessment.Title} is due on {assessment.StartDate:MMM dd}",
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = assessment.StartDate.Date.AddHours(9),
                    RepeatType = NotificationRepeat.No
                }
            });
        }

        private async Task ScheduleCourseNotifications(Course course)
        {
            if (!courseNotifySwitch.IsToggled) return;

            await LocalNotificationCenter.Current.Show(new NotificationRequest
            {
                NotificationId = new Random().Next(1000, 1999),
                Title = "Course Starting",
                Description = $"Your course '{course.CourseTitle}' starts today.",
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = course.StartDate.Date.AddHours(9)
                }
            });

            await LocalNotificationCenter.Current.Show(new NotificationRequest
            {
                NotificationId = new Random().Next(2000, 2999),
                Title = "Course Ending",
                Description = $"Your course '{course.CourseTitle}' ends today.",
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = course.EndDate.Date.AddHours(9)
                }
            });
        }

        private async void OnSaveCourseClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(courseTitleEntry.Text))
            {
                await DisplayAlert("Validation Error", "Please enter a course title.", "OK");
                return;
            }

            var newCourse = new Course
            {
                CourseTitle = courseTitleEntry.Text.Trim(),
                StartDate = startDatePicker.Date,
                EndDate = endDatePicker.Date,
                NotifyOnStart = courseNotifySwitch.IsToggled,
                NotifyOnEnd = courseNotifySwitch.IsToggled,
                InstructorName = instructorNameEntry.Text?.Trim(),
                InstructorEmail = instructorEmailEntry.Text?.Trim(),
                InstructorPhone = instructorPhoneEntry.Text?.Trim(),
                Notes = notesEditor.Text?.Trim(),
                CourseStatus = statusPicker.SelectedItem?.ToString(),
                TermId = _term.Id
            };

            await _db.SaveCourseAsync(newCourse);
            await ScheduleCourseNotifications(newCourse);

            if (!string.IsNullOrWhiteSpace(performanceTitleEntry.Text))
            {
                var perfAssessment = new Assessment
                {
                    CourseId = newCourse.Id,
                    Title = performanceTitleEntry.Text.Trim(),
                    StartDate = performanceDueDatePicker.Date,
                    EndDate = performanceDueDatePicker.Date,
                    Type = "Performance"
                };

                await _db.SaveAssessmentAsync(perfAssessment);
                await ScheduleAssessmentNotification(perfAssessment);
            }

            if (!string.IsNullOrWhiteSpace(objectiveTitleEntry.Text))
            {
                var objAssessment = new Assessment
                {
                    CourseId = newCourse.Id,
                    Title = objectiveTitleEntry.Text.Trim(),
                    StartDate = objectiveDueDatePicker.Date,
                    EndDate = objectiveDueDatePicker.Date,
                    Type = "Objective"
                };

                await _db.SaveAssessmentAsync(objAssessment);
                await ScheduleAssessmentNotification(objAssessment);
            }

            await DisplayAlert("Success", "Course added.", "OK");
            await Navigation.PopAsync();
        }

        private void OnClearTapped(object sender, EventArgs e)
        {
            courseTitleEntry.Text = string.Empty;
            startDatePicker.Date = DateTime.Now;
            endDatePicker.Date = DateTime.Now;
            statusPicker.SelectedIndex = -1;

            performanceTitleEntry.Text = string.Empty;
            performanceDueDatePicker.Date = DateTime.Now;

            objectiveTitleEntry.Text = string.Empty;
            objectiveDueDatePicker.Date = DateTime.Now;

            instructorNameEntry.Text = string.Empty;
            instructorEmailEntry.Text = string.Empty;
            instructorPhoneEntry.Text = string.Empty;

            notesEditor.Text = string.Empty;

            courseNotifySwitch.IsToggled = false;
            assessmentNotifySwitch.IsToggled = false;
        }
    }
}
