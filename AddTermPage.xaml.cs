using MobileApplicationDev.Models;
using MobileApplicationDev.Services;

namespace MobileApplicationDev;

public partial class AddTermPage : ContentPage
{
    private readonly DatabaseService _db;

    public AddTermPage(DatabaseService db)
    {
        InitializeComponent();
        _db = db;
    }

    private void OnAddCourseClicked(object sender, EventArgs e)
    {
        var titleLabel = new Label
        {
            Text = "Course Title:",
            FontAttributes = FontAttributes.Bold
        };

        var titleEntry = new Entry
        {
            Placeholder = "Enter course title"
        };

        var startDateLabel = new Label
        {
            Text = "Start Date:",
            FontAttributes = FontAttributes.Bold
        };

        var startDatePicker = new DatePicker
        {
            Format = "MM/dd/yyyy"
        };

        var endDateLabel = new Label
        {
            Text = "End Date:",
            FontAttributes = FontAttributes.Bold
        };

        var endDatePicker = new DatePicker
        {
            Format = "MM/dd/yyyy"
        };

        // Performance Assessment
        var perfLabel = new Label
        {
            Text = "Performance Assessment:",
            FontAttributes = FontAttributes.Bold
        };

        var perfEntry = new Entry
        {
            Placeholder = "Enter title"
        };

        var perfDueLabel = new Label
        {
            Text = "Due Date:"
        };

        var perfDueDate = new DatePicker
        {
            Format = "MM/dd/yyyy"
        };

        // Objective Assessment
        var objLabel = new Label
        {
            Text = "Objective Assessment:",
            FontAttributes = FontAttributes.Bold
        };

        var objEntry = new Entry
        {
            Placeholder = "Enter title"
        };

        var objDueLabel = new Label
        {
            Text = "Due Date:"
        };

        var objDueDate = new DatePicker
        {
            Format = "MM/dd/yyyy"
        };

        // Notes
        var notesLabel = new Label
        {
            Text = "Notes:",
            FontAttributes = FontAttributes.Bold
        };

        var notesEditor = new Editor
        {
            HeightRequest = 80,
            Placeholder = "Optional notes"
        };

        var deleteButton = new Button
        {
            Text = "🗑",
            BackgroundColor = Colors.LightGray,
            WidthRequest = 60,
            HorizontalOptions = LayoutOptions.End
        };

        var innerLayout = new VerticalStackLayout
        {
            Spacing = 8,
            Children =
        {
            titleLabel,
            titleEntry,
            startDateLabel,
            startDatePicker,
            endDateLabel,
            endDatePicker,
            perfLabel,
            perfEntry,
            perfDueLabel,
            perfDueDate,
            objLabel,
            objEntry,
            objDueLabel,
            objDueDate,
            notesLabel,
            notesEditor,
            deleteButton
        }
        };

        var courseFrame = new Frame
        {
            BorderColor = Colors.Gray,
            Padding = 10,
            Margin = new Thickness(0, 10),
            Content = innerLayout
        };

        deleteButton.Clicked += (s, args) =>
        {
            courseListLayout.Children.Remove(courseFrame);
        };

        courseListLayout.Children.Add(courseFrame);
    }


    private async void OnAddTermClicked(object sender, EventArgs e)
    {
        var term = new Term
        {
            Title = termTitleEntry.Text,
            StartDate = termStartDate.Date,
            EndDate = termEndDate.Date
        };

        await _db.SaveTermAsync(term);

        foreach (var child in courseListLayout.Children)
        {
            if (child is Frame frame && frame.Content is VerticalStackLayout layout)
            {
                var titleEntry = layout.Children[1] as Entry;
                var startDate = (layout.Children[3] as DatePicker)?.Date ?? DateTime.Now;
                var endDate = (layout.Children[5] as DatePicker)?.Date ?? DateTime.Now;
                var notesEditor = layout.Children[15] as Editor;

                var course = new Course
                {
                    CourseTitle = titleEntry?.Text ?? "Untitled",
                    StartDate = startDate,
                    EndDate = endDate,
                    Notes = notesEditor?.Text ?? "",
                    TermId = term.Id
                };

                await _db.SaveCourseAsync(course);

                // Optional: handle assessments using layout.Children[7]–[13]
            }
        }

        await DisplayAlert("Saved", "Term and courses have been saved.", "OK");
        await Navigation.PopAsync();
    }
}

