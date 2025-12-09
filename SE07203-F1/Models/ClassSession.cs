using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SE07203_F1.Models
{
    public class ClassSession
    {
        public int Id { get; set; }

        public DateTime Date { get; set; } // Ngày học cụ thể (VD: 2023-12-05)

        public int Slot { get; set; } // Ca học (1, 2, 3, 4...)

        public string Room { get; set; } // Phòng học

        // Liên kết với Lớp học phần
        public int ClassSectionId { get; set; }
        public ClassSection ClassSection { get; set; }

        // 💡 ĐÃ XÓA: ICollection<ClassSession> ClassSessions (Không cần thiết ở đây)
    }
}