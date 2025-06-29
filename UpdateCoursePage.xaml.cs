using System;
using Microsoft.Maui.Controls;

namespace MobileApplicationDev
{
    public partial class UpdateCoursePage : ContentPage
    {
        public UpdateCoursePage(string courseTitle, DateTime startDate, DateTime endDate)
        {
            InitializeComponent();

            // Set existing course values
            courseTitleEntry.Text = courseTitle;
            startDatePicker.Date = startDate;
            endDatePicker.Date = endDate;

            // Optional: Pre-fill assessment fields
            performanceAssessmentEntry.Text = "";
            performanceDueDatePicker.Date = endDate;

            objectiveAssessmentEntry.Text = "";
            objectiveDueDatePicker.Date = endDate;
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            // Collect course details
            string courseTitle = courseTitleEntry.Text;
            DateTime startDate = startDatePicker.Date;
            DateTime endDate = endDatePicker.Date;

            // Collect assessments
            string performanceTitle = performanceAssessmentEntry.Text;
            DateTime performanceDueDate = performanceDueDatePicker.Date;

            string objectiveTitle = objectiveAssessmentEntry.Text;
            DateTime objectiveDueDate = objectiveDueDatePicker.Date;

            // (Optional) Do something with the data here...

            await DisplayAlert("Saved", $"Course '{courseTitle}' updated successfully.", "OK");
            await Navigation.PopAsync();
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
