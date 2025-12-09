namespace SE07203_F1.Models
{
    public class ClassSectionEditViewModel
    {
        // 1. Thông tin lớp học (Từ ClassSection)
        public int Id { get; set; } // Phải có Id
        public string Code { get; set; }
        public string Room { get; set; }
        public int CourseId { get; set; }
        public int LecturerId { get; set; }

        // 2. Thông tin Xếp lịch (Từ ClassScheduleViewModel)
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Slot { get; set; }
        public int[] DaysOfWeek { get; set; } // Danh sách các thứ trong tuần
    }
}