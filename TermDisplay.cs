using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MobileApplicationDev.Models;

namespace MobileApplicationDev.Models
{
    public class TermDisplay
    {
        public string Title { get; set; }
        public string StartDateText { get; set; }
        public string EndDateText { get; set; }

        public List<Course> Courses { get; set; }

        public Term TermObject { get; set; }
    }

}

