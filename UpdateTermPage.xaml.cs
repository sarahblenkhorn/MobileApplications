using System;
using Microsoft.Maui.Controls;
using MobileApplicationDev.Models;
using MobileApplicationDev.Services;

namespace MobileApplicationDev
{
    public partial class UpdateTermPage : ContentPage
    {
        private readonly Term _term;
        private readonly DatabaseService _db;

        public UpdateTermPage(Term term, DatabaseService db)
        {
            InitializeComponent();
            _term = term;
            _db = db;

            // Populate UI with term data
            termTitleEntry.Text = _term.Title;
            termStartDate.Date = _term.StartDate;
            termEndDate.Date = _term.EndDate;
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            // Validate and update term data
            _term.Title = termTitleEntry.Text?.Trim() ?? "Untitled Term";
            _term.StartDate = termStartDate.Date;
            _term.EndDate = termEndDate.Date;

            await _db.SaveTermAsync(_term);
            await DisplayAlert("Saved", "Term updated successfully.", "OK");
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
}
