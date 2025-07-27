using MobileApplicationDev.Models;
using MobileApplicationDev.Services;
using Plugin.LocalNotification;

namespace MobileApplicationDev;

public partial class UpdateCoursePage : ContentPage
{
    private readonly DatabaseService _db;
    private readonly Course _course;
    private Assessment _performanceAssessment;
    private Assessment _objectiveAssessment;

    public UpdateCoursePage(Course course, DatabaseService db, Assessment performanceAssessment, Assessment objectiveAssessment)
    {
        InitializeComponent();
        _db = db;
        _course = course;
        _performanceAssessment = performanceAssessment;
        _objectiveAssessment = objectiveAssessment;

        // Populate fields
        courseTitleEntry.Text = _course.CourseTitle;
        startDatePicker.Date = _course.StartDate;
        endDatePicker.Date = _course.EndDate;

        if (_performanceAssessment != null)
        {
            performanceAssessmentEntry.Text = _performanceAssessment.Title;
            performanceDueDatePicker.Date = _performanceAssessment.StartDate;
        }

        if (_objectiveAssessment != null)
        {
            objectiveAssessmentEntry.Text = _objectiveAssessment.Title;
            objectiveDueDatePicker.Date = _objectiveAssessment.StartDate;
        }

        instructorNameEntry.Text = _course.InstructorName;
        instructorEmailEntry.Text = _course.InstructorEmail;
        instructorPhoneEntry.Text = _course.InstructorPhone;
        notesEditor.Text = _course.Notes;
        statusPicker.ItemsSource = new List<string> { "In Progress", "Completed", "Dropped", "Plan to Take" };
        statusPicker.SelectedItem = _course.CourseStatus;

        courseNotifySwitch.IsToggled = _course.NotifyOnStart;
        assessmentNotifySwitch.IsToggled = true; // Assume true if any assessment exists
    }

    private async Task ScheduleCourseNotifications()
    {
        if (courseNotifySwitch.IsToggled)
        {
            await LocalNotificationCenter.Current.Show(new NotificationRequest
            {
                NotificationId = _course.Id + 1000,
                Title = "Course Starting",
                Description = $"'{_course.CourseTitle}' starts today.",
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = _course.StartDate.Date.AddHours(9),
                    RepeatType = NotificationRepeat.No
                }
            });

            await LocalNotificationCenter.Current.Show(new NotificationRequest
            {
                NotificationId = _course.Id + 2000,
                Title = "Course Ending",
                Description = $"'{_course.CourseTitle}' ends today.",
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = _course.EndDate.Date.AddHours(9),
                    RepeatType = NotificationRepeat.No
                }
            });
        }
    }

    private async Task ScheduleAssessmentNotifications()
    {
        if (assessmentNotifySwitch.IsToggled)
        {
            if (_performanceAssessment != null)
            {
                await LocalNotificationCenter.Current.Show(new NotificationRequest
                {
                    NotificationId = _performanceAssessment.Id + 3000,
                    Title = "Performance Assessment Due",
                    Description = $"{_performanceAssessment.Title} is due on {_performanceAssessment.StartDate:MMM dd}",
                    Schedule = new NotificationRequestSchedule
                    {
                        NotifyTime = _performanceAssessment.StartDate.Date.AddHours(9),
                        RepeatType = NotificationRepeat.No
                    }
                });
            }

            if (_objectiveAssessment != null)
            {
                await LocalNotificationCenter.Current.Show(new NotificationRequest
                {
                    NotificationId = _objectiveAssessment.Id + 4000,
                    Title = "Objective Assessment Due",
                    Description = $"{_objectiveAssessment.Title} is due on {_objectiveAssessment.StartDate:MMM dd}",
                    Schedule = new NotificationRequestSchedule
                    {
                        NotifyTime = _objectiveAssessment.StartDate.Date.AddHours(9),
                        RepeatType = NotificationRepeat.No
                    }
                });
            }
        }
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        string email = instructorEmailEntry.Text?.Trim() ?? "";
        if (string.IsNullOrWhiteSpace(email) || !email.Contains("@") || !email.Contains("."))
        {
            await DisplayAlert("Validation Error", "Please enter a valid instructor email.", "OK");
            return;
        }

        string phone = instructorPhoneEntry.Text?.Trim() ?? "";
        if (string.IsNullOrWhiteSpace(phone) || phone.Length < 7)
        {
            await DisplayAlert("Validation Error", "Please enter a valid instructor phone number.", "OK");
            return;
        }

        if (_performanceAssessment != null)
        {
            _performanceAssessment.Title = performanceAssessmentEntry.Text ?? "";
            _performanceAssessment.StartDate = performanceDueDatePicker.Date;
            await _db.SaveAssessmentAsync(_performanceAssessment);
        }

        if (_objectiveAssessment != null)
        {
            _objectiveAssessment.Title = objectiveAssessmentEntry.Text ?? "";
            _objectiveAssessment.StartDate = objectiveDueDatePicker.Date;
            await _db.SaveAssessmentAsync(_objectiveAssessment);
        }

        _course.CourseTitle = courseTitleEntry.Text ?? "Untitled";
        _course.CourseStatus = statusPicker.SelectedItem?.ToString() ?? "In Progress";
        _course.StartDate = startDatePicker.Date;
        _course.EndDate = endDatePicker.Date;
        _course.PerformanceAssessment = _performanceAssessment?.Title ?? "";
        _course.PerformanceDueDate = _performanceAssessment?.StartDate ?? DateTime.MinValue;
        _course.ObjectiveAssessment = _objectiveAssessment?.Title ?? "";
        _course.ObjectiveDueDate = _objectiveAssessment?.StartDate ?? DateTime.MinValue;
        _course.InstructorName = instructorNameEntry.Text ?? "";
        _course.InstructorEmail = email;
        _course.InstructorPhone = phone;
        _course.Notes = notesEditor.Text ?? "";
        _course.NotifyOnStart = courseNotifySwitch.IsToggled;
        _course.NotifyOnEnd = courseNotifySwitch.IsToggled;

        await _db.SaveCourseAsync(_course);
        await ScheduleCourseNotifications();
        await ScheduleAssessmentNotifications();

        await DisplayAlert("Saved", "Course updated successfully.", "OK");
        await Navigation.PopAsync();
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("Cancel", "Discard changes?", "Yes", "No");
        if (confirm)
        {
            await Navigation.PopAsync();
        }
    }
}
