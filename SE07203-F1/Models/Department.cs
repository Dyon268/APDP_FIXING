using System.ComponentModel.DataAnnotations;

namespace SE07203_F1.Models
{
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // Navigation Properties (Liên kết 1-nhiều)
        public ICollection<Student> Students { get; set; } = new List<Student>();
        public ICollection<Lecturer> Lecturers { get; set; } = new List<Lecturer>();
    }
}
