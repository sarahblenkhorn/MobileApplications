using System;
using System.Collections.Generic;
using SQLite;

namespace MobileApplicationDev.Models
{
    public class Course
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string CourseTitle { get; set; } = string.Empty;
        public string CourseStatus { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string InstructorName { get; set; } = string.Empty;
        public string InstructorEmail { get; set; } = string.Empty;
        public string InstructorPhone { get; set; } = string.Empty;

        public string Notes { get; set; } = string.Empty;

        public string PerformanceAssessment { get; set; } = string.Empty;
        public DateTime PerformanceDueDate { get; set; }

        public string ObjectiveAssessment { get; set; } = string.Empty;
        public DateTime ObjectiveDueDate { get; set; }

        public int TermId { get; set; }

        [Ignore]
        public List<Assessment> Assessments { get; set; } = new();

        public string DateRange => $"{StartDate:MMM dd} - {EndDate:MMM dd}";

        public bool NotifyOnStart { get; set; }
        public bool NotifyOnEnd { get; set; }
    }
}




