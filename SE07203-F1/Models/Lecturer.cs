using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SE07203_F1.Models
{
    public class Lecturer
    {
        public int Id { get; set; }
        public string Fullname { get; set; }

        // Khóa ngoại
        public int AccountId { get; set; }
        public int DepartmentId { get; set; }

        // Navigation Properties
        public Account Account { get; set; }
        public Department Department { get; set; }
        public ICollection<ClassSection> ClassSections { get; set; } = new List<ClassSection>();
    }
}
