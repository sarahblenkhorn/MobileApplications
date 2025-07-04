using Microsoft.Maui.Controls;
using MobileApplicationDev.Models;
using MobileApplicationDev.Services;
using System;
using System.Collections.ObjectModel;

namespace MobileApplicationDev
{
    public partial class MainPage : ContentPage
    {
        private readonly DatabaseService _db;
        public ObservableCollection<Course> Courses { get; set; }

        public MainPage(DatabaseService db)
        {
            InitializeComponent();
            _db = db;
            Courses = new ObservableCollection<Course>();
            BindingContext = this;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            Courses.Clear();
            var allTerms = await _db.GetTermsAsync();

            foreach (var term in allTerms)
            {
                var courses = await _db.GetCoursesForTermAsync(term.Id);
                foreach (var course in courses)
                {
                    Courses.Add(course);
                }
            }
        }

        private async void OnAddButtonClicked(object sender, EventArgs e)
        {
            var addTermPage = App.Services.GetService<AddTermPage>();
            await Navigation.PushAsync(addTermPage);
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







