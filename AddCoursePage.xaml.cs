using Microsoft.Maui.Controls;
using MobileApplicationDev.Models;
using MobileApplicationDev.Services;
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

            // Save assessments
            if (!string.IsNullOrWhiteSpace(performanceTitleEntry.Text))
            {
                await _db.SaveAssessmentAsync(new Assessment
                {
                    CourseId = newCourse.Id,
                    Title = performanceTitleEntry.Text.Trim(),
                    StartDate = performanceDueDatePicker.Date,
                    Type = "Performance",
                });
            }

            if (!string.IsNullOrWhiteSpace(objectiveTitleEntry.Text))
            {
                await _db.SaveAssessmentAsync(new Assessment
                {
                    CourseId = newCourse.Id,
                    Title = objectiveTitleEntry.Text.Trim(),
                    StartDate = objectiveDueDatePicker.Date,
                    Type = "Objective",
                });
            }

            await DisplayAlert("Success", "Course added.", "OK");
            await Navigation.PopAsync();
        }
    }
}
