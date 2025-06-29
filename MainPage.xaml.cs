using System;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;
using MobileApplicationDev.Models;

namespace MobileApplicationDev
{
    public partial class MainPage : ContentPage
    {
        public ObservableCollection<Course> Courses { get; set; }

        public MainPage()
        {
            InitializeComponent();

            Courses = new ObservableCollection<Course>
            {
                new Course
                {
                    CourseTitle = "C# Programming",
                    CourseStatus = "In Progress",
                    StartDate = new DateTime(2025, 6, 1),
                    EndDate = new DateTime(2025, 9, 1),
                    InstructorName = "Dr. Jane Doe",
                    InstructorEmail = "jane@example.com",
                    InstructorPhone = "555-1234",
                    Notes = "Covering advanced topics"
                },
                new Course
                {
                    CourseTitle = "Databases 101",
                    CourseStatus = "Planned",
                    StartDate = new DateTime(2025, 9, 15),
                    EndDate = new DateTime(2025, 12, 15),
                    InstructorName = "John Smith",
                    InstructorEmail = "john@example.com",
                    InstructorPhone = "555-5678",
                    Notes = "Start with normalization"
                }
            };

            BindingContext = this;
        }

        private async void OnAddButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddTermPage());
        }

        private async void OnUpdateTermTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new UpdateTermPage());
        }

        private async void OnCourseTapped(object sender, EventArgs e)
        {
            if (sender is VisualElement view && view.BindingContext is Course course)
            {
                await Navigation.PushAsync(new CourseDetailPage
                {
                    BindingContext = course
                });
            }
        }
    }
}






