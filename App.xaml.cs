using MobileApplicationDev.Models;
using MobileApplicationDev.Services;

namespace MobileApplicationDev
{
    public partial class App : Application
    {
        public static IServiceProvider Services { get; private set; }

        public App(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            Services = serviceProvider;

            MainPage = new AppShell();

            Task.Run(async () => await SeedEvaluationDataAsync());
        }

        private async Task SeedEvaluationDataAsync()
        {
            var db = new DatabaseService();

            var existingTerms = await db.GetTermsAsync();
            if (existingTerms.Any(t => t.Title == "Evaluation Term"))
                return;

            var term = new Term
            {
                Title = "Evaluation Term",
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddMonths(3)
            };
            await db.SaveTermAsync(term);

            var course = new Course
            {
                CourseTitle = "Evaluation Course",
                CourseStatus = "In Progress",
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddMonths(2),
                InstructorName = "Anika Patel",
                InstructorPhone = "555-123-4567",
                InstructorEmail = "anika.patel@strimeuniversity.edu",
                Notes = "Seeded course for evaluation",
                NotifyOnStart = false,
                NotifyOnEnd = false,
                TermId = term.Id
            };
            await db.SaveCourseAsync(course);

            await db.SaveAssessmentAsync(new Assessment
            {
                CourseId = course.Id,
                Title = "Performance Evaluation",
                StartDate = DateTime.Today.AddDays(7),
                EndDate = DateTime.Today.AddDays(7),
                Type = "Performance"
            });

            await db.SaveAssessmentAsync(new Assessment
            {
                CourseId = course.Id,
                Title = "Objective Evaluation",
                StartDate = DateTime.Today.AddDays(14),
                EndDate = DateTime.Today.AddDays(14),
                Type = "Objective"
            });
        }
    }
}