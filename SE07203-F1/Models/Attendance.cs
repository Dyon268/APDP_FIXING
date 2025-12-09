namespace SE07203_F1.Models
{
    public class Attendance
    {
        public int AttendanceId { get; set; }
        public DateTime AttendDate { get; set; }
        public string Status { get; set; }

        public int EnrollmentId { get; set; }
        public Enrollment Enrollment { get; set; }
    }

}
