namespace MobileApplicationDev
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnAddButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddTermPage());
        }

        private async void OnUpdateCourseClicked(object sender, EventArgs e)
        {
            // Example course data passed to UpdateCoursePage
            string courseTitle = "C# Programming";
            DateTime startDate = new DateTime(2025, 6, 1);
            DateTime endDate = new DateTime(2025, 9, 1);

            await Navigation.PushAsync(new UpdateCoursePage(courseTitle, startDate, endDate));
        }

        private async void OnUpdateTermTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new UpdateTermPage());
        }
    }
}



