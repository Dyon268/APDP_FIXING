using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SE07203_F1.Models
{
    public class Enrollment
    {
        public int Id { get; set; }

        // Khóa ngoại
        public int StudentId { get; set; }
        public int ClassSectionId { get; set; }

        // Navigation Properties
        public Student Student { get; set; }
        public ClassSection ClassSection { get; set; }
    }
}
