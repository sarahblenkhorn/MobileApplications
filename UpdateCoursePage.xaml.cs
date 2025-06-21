namespace MobileApplicationDev;

public partial class UpdateCoursePage : ContentPage
{
    public UpdateCoursePage(string title = "", DateTime? start = null, DateTime? end = null)
    {
        InitializeComponent();

        // Pre-fill values
        titleEntry.Text = title;
        startDatePicker.Date = start ?? DateTime.Now;
        endDatePicker.Date = end ?? DateTime.Now;
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        // TODO: save logic here
        await DisplayAlert("Saved", "Course updated successfully.", "OK");
        await Navigation.PopAsync();
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
