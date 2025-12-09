namespace SE07203_F1.Models
{
    public class AttendanceViewModel
    {
        public int SessionId { get; set; }
        public DateTime Date { get; set; }
        public int Slot { get; set; }
        public List<AttendanceItem> Items { get; set; } = new List<AttendanceItem>();
    }

    public class AttendanceItem
    {
        public int EnrollmentId { get; set; }
        public string StudentName { get; set; }
        public string StudentCode { get; set; }
        public string Status { get; set; } // Sẽ nhận giá trị: Present, Absent, Late
    }
}