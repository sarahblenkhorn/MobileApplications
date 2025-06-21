namespace MobileApplicationDev;

public partial class UpdateTermPage : ContentPage
{
    public UpdateTermPage()
    {
        InitializeComponent();
    }


    private void OnAddCourseClicked(object sender, EventArgs e)
    {
        var courseTitle = new Entry
        {
            Placeholder = "Course Title",
            WidthRequest = 300
        };

        var startDateLabel = new Label
        {
            Text = "Start Date:",
            VerticalTextAlignment = TextAlignment.Center
        };
        var startDatePicker = new DatePicker
        {
            Format = "MM/dd/yyyy",
            WidthRequest = 200
        };

        var endDateLabel = new Label
        {
            Text = "End Date:",
            VerticalTextAlignment = TextAlignment.Center
        };
        var endDatePicker = new DatePicker
        {
            Format = "MM/dd/yyyy",
            WidthRequest = 200
        };

        var deleteButton = new Button
        {
            Text = "🗑",
            BackgroundColor = Colors.LightGray,
            WidthRequest = 50,
            HorizontalOptions = LayoutOptions.End
        };

        var courseBlock = new Frame
        {
            Padding = 10,
            Margin = new Thickness(0, 5),
            CornerRadius = 10,
            BorderColor = Colors.LightGray,
            Content = new VerticalStackLayout
            {
                Spacing = 8,
                Children =
                {
                    courseTitle,
                    new HorizontalStackLayout { Children = { startDateLabel, startDatePicker } },
                    new HorizontalStackLayout { Children = { endDateLabel, endDatePicker } },
                    deleteButton
                }
            }
        };

        deleteButton.Clicked += (s, args) =>
        {
            courseListLayout.Children.Remove(courseBlock);
        };

        courseListLayout.Children.Add(courseBlock);
    }

    private void OnSaveChangesClicked(object sender, EventArgs e)
    {
        DisplayAlert("Saved", "Term changes have been saved.", "OK");
    }
}
