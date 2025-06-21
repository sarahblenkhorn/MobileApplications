namespace MobileApplicationDev;

public partial class AddTermPage : ContentPage
{
    public AddTermPage()
    {
        InitializeComponent();
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


    private void OnAddTermClicked(object sender, EventArgs e)
    {
        DisplayAlert("Saved", "Term has been added.", "OK");
    }
}
