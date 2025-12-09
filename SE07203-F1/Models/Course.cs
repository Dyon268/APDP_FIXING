using System.ComponentModel.DataAnnotations;

namespace SE07203_F1.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Code { get; set; } // Mã môn học (ví dụ: IT101)
        public string Name { get; set; }

        // Navigation Property
        public ICollection<ClassSection> ClassSections { get; set; } = new List<ClassSection>();
    }
}
