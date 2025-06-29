using System;
using Microsoft.Maui.Controls;

namespace MobileApplicationDev
{
    public partial class UpdateTermPage : ContentPage
    {
        public UpdateTermPage()
        {
            InitializeComponent();
        }

        private void OnAddCourseClicked(object sender, EventArgs e)
        {
            var titleLabel = new Label { Text = "Course Title:", FontAttributes = FontAttributes.Bold };
            var titleEntry = new Entry { Placeholder = "Enter course title" };

            var startDateLabel = new Label { Text = "Start Date:", FontAttributes = FontAttributes.Bold };
            var startDatePicker = new DatePicker { Format = "MM/dd/yyyy" };

            var endDateLabel = new Label { Text = "End Date:", FontAttributes = FontAttributes.Bold };
            var endDatePicker = new DatePicker { Format = "MM/dd/yyyy" };

            var performanceLabel = new Label { Text = "Performance Assessment:", FontAttributes = FontAttributes.Bold };
            var performanceEntry = new Entry { Placeholder = "Enter title" };

            var performanceDueLabel = new Label { Text = "Due Date:", FontAttributes = FontAttributes.Bold };
            var performanceDuePicker = new DatePicker { Format = "MM/dd/yyyy" };

            var objectiveLabel = new Label { Text = "Objective Assessment:", FontAttributes = FontAttributes.Bold };
            var objectiveEntry = new Entry { Placeholder = "Enter title" };

            var objectiveDueLabel = new Label { Text = "Due Date:", FontAttributes = FontAttributes.Bold };
            var objectiveDuePicker = new DatePicker { Format = "MM/dd/yyyy" };

            var notesLabel = new Label { Text = "Notes:", FontAttributes = FontAttributes.Bold };
            var notesEditor = new Editor { AutoSize = EditorAutoSizeOption.TextChanges, Placeholder = "Optional notes" };

            var deleteButton = new Button
            {
                Text = "🗑",
                BackgroundColor = Colors.LightGray,
                WidthRequest = 60,
                HorizontalOptions = LayoutOptions.End
            };

            var courseStack = new VerticalStackLayout
            {
                Spacing = 8,
                Children =
                {
                    titleLabel, titleEntry,
                    startDateLabel, startDatePicker,
                    endDateLabel, endDatePicker,
                    performanceLabel, performanceEntry,
                    performanceDueLabel, performanceDuePicker,
                    objectiveLabel, objectiveEntry,
                    objectiveDueLabel, objectiveDuePicker,
                    notesLabel, notesEditor,
                    deleteButton
                }
            };

            var courseFrame = new Frame
            {
                BorderColor = Colors.Gray,
                Padding = 10,
                Margin = new Thickness(0, 10),
                Content = courseStack
            };

            deleteButton.Clicked += (s, args) =>
            {
                courseListLayout.Children.Remove(courseFrame);
            };

            courseListLayout.Children.Add(courseFrame);
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            // Here you'd collect and save the updated term and course details
            await DisplayAlert("Updated", "Term and courses have been updated.", "OK");
            await Navigation.PopAsync();
        }
    }
}
