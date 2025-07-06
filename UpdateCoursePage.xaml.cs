using MobileApplicationDev.Models;
using MobileApplicationDev.Services;

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

        // Load course data
        courseTitleEntry.Text = _course.CourseTitle;
        startDatePicker.Date = _course.StartDate;
        endDatePicker.Date = _course.EndDate;

        if (performanceAssessment != null)
        {
            performanceAssessmentEntry.Text = performanceAssessment.Title;
            performanceDueDatePicker.Date = performanceAssessment.StartDate;
        }

        if (objectiveAssessment != null)
        {
            objectiveAssessmentEntry.Text = objectiveAssessment.Title;
            objectiveDueDatePicker.Date = objectiveAssessment.StartDate;
        }

        instructorNameEntry.Text = _course.InstructorName;
        instructorEmailEntry.Text = _course.InstructorEmail;
        instructorPhoneEntry.Text = _course.InstructorPhone;
        notesEditor.Text = _course.Notes;
        statusPicker.SelectedItem = _course.CourseStatus;
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

        // Update assessment objects
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

        // Update course fields
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

        await _db.SaveCourseAsync(_course);
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

