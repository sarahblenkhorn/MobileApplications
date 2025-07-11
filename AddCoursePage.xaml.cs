using Microsoft.Maui.Controls;
using MobileApplicationDev.Models;
using MobileApplicationDev.Services;
using Plugin.LocalNotification;
using System;

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
        }

        private async Task ScheduleAssessmentNotification(Assessment assessment)
        {
            await LocalNotificationCenter.Current.Show(new NotificationRequest
            {
                NotificationId = assessment.Id + 3000,
                Title = $"{assessment.Type} Assessment Due",
                Description = $"{assessment.Title} is due on {assessment.StartDate:MMM dd}",
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = assessment.StartDate,
                    RepeatType = NotificationRepeat.No
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
                InstructorName = instructorNameEntry.Text?.Trim(),
                InstructorEmail = instructorEmailEntry.Text?.Trim(),
                InstructorPhone = instructorPhoneEntry.Text?.Trim(),
                Notes = notesEditor.Text?.Trim(),
                CourseStatus = statusPicker.SelectedItem?.ToString(),
                TermId = _term.Id
            };

            await _db.SaveCourseAsync(newCourse);

            await LocalNotificationCenter.Current.Show(new NotificationRequest
            {
                NotificationId = new Random().Next(1000, 1999),
                Title = "Course Starting",
                Description = $"Your course '{newCourse.CourseTitle}' starts today.",
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = newCourse.StartDate.Date.AddHours(9)
                }
            });


            // 🔔 Notify for course end
            await LocalNotificationCenter.Current.Show(new NotificationRequest
            {
                NotificationId = new Random().Next(2000, 2999),
                Title = "Course Ending",
                Description = $"Your course '{newCourse.CourseTitle}' ends today.",
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = newCourse.EndDate.Date.AddHours(9)
                }
            });

            // 🔁 Save and notify for Performance Assessment
            if (!string.IsNullOrWhiteSpace(performanceTitleEntry.Text))
            {
                var perfAssessment = new Assessment
                {
                    CourseId = newCourse.Id,
                    Title = performanceTitleEntry.Text.Trim(),
                    StartDate = performanceDueDatePicker.Date,
                    EndDate = performanceDueDatePicker.Date,
                    Type = "Performance",
                };

                await _db.SaveAssessmentAsync(perfAssessment);
                await ScheduleAssessmentNotification(perfAssessment);
            }

            // 🔁 Save and notify for Objective Assessment
            if (!string.IsNullOrWhiteSpace(objectiveTitleEntry.Text))
            {
                var objAssessment = new Assessment
                {
                    CourseId = newCourse.Id,
                    Title = objectiveTitleEntry.Text.Trim(),
                    StartDate = objectiveDueDatePicker.Date,
                    EndDate = objectiveDueDatePicker.Date,
                    Type = "Objective",
                };

                await _db.SaveAssessmentAsync(objAssessment);
                await ScheduleAssessmentNotification(objAssessment);
            }

            await DisplayAlert("Success", "Course added.", "OK");
            await Navigation.PopAsync();
        }
    }
}