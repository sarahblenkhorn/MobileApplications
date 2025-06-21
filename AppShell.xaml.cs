namespace MobileApplicationDev
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(AddTermPage), typeof(AddTermPage));
            Routing.RegisterRoute(nameof(UpdateTermPage), typeof(UpdateTermPage));
            Routing.RegisterRoute(nameof(UpdateCoursePage), typeof(UpdateCoursePage));
        }
    }
}

