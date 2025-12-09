using System.ComponentModel.DataAnnotations;

namespace SE07203_F1.Models
{
    public class CreateClassViewModel
    {
        // --- Thông tin cho bảng ClassSection ---
        public string Code { get; set; } // Mã lớp
        public string Room { get; set; } // Phòng học
        public int CourseId { get; set; }
        public int LecturerId { get; set; }

        // --- Thông tin để chạy vòng lặp xếp lịch (ClassSession) ---
        public DateTime StartDate { get; set; } // Ngày bắt đầu
        public DateTime EndDate { get; set; }   // Ngày kết thúc
        public int Slot { get; set; }           // Ca học (1, 2, 3...)

        // Danh sách các thứ trong tuần được chọn (0=CN, 1=Thứ 2, 2=Thứ 3...)
        public List<int> DaysOfWeek { get; set; } = new List<int>();
    }
}