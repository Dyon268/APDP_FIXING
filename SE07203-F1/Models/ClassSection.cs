using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SE07203_F1.Models
{
    public class ClassSection
    {
        public int Id { get; set; }
        public string Code { get; set; } // Mã lớp (ví dụ: IT101_K65_T1)
        public string Room { get; set; } // Phòng học

        // Khóa ngoại
        public int CourseId { get; set; }
        public int LecturerId { get; set; }

        // Navigation Properties
        public Course Course { get; set; }
        public Lecturer Lecturer { get; set; }
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public ICollection<ClassSession> ClassSessions { get; set; } = new List<ClassSession>();
    }
}
