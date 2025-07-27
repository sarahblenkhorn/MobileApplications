using System;
using SQLite;

namespace MobileApplicationDev.Models
{
    public class Assessment
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int CourseId { get; set; }

        public string Type { get; set; } = string.Empty; 

        public string DisplayLabel => $"{Type} Assessment";

        public string DueDateText => $"Due: {StartDate:MMM dd}";

    }
}



