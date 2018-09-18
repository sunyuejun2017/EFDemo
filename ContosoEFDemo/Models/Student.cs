using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ContosoEFDemo.Models
{
    public class Student
    {
        public int ID { get; set; }
        public string LastName { get; set; }
        public string FirstMidName { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public int stu_Number { get; set; }
        public string UpdateDate { get; set; }

        public virtual ICollection<Enrollment> Enrollments { get; set; }
    }
}