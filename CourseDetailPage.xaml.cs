using System;
using Microsoft.Maui.Controls;
using MobileApplicationDev.Models;

namespace MobileApplicationDev
{
    public partial class CourseDetailPage : ContentPage
    {
        public CourseDetailPage()
        {
            InitializeComponent();
        }

        private async void OnUpdateCourseClicked(object sender, EventArgs e)
        {
            if (BindingContext is Course course)
            {
                await Navigation.PushAsync(new UpdateCoursePage(course.CourseTitle, course.StartDate, course.EndDate));
            }
        }
    }
}

