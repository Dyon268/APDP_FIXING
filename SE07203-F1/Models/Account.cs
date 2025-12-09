using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SE07203_F1.Models
{
    public class Account
    {
        public int Id { get; set; }
        public string Fullname { get; set; } // Tên đầy đủ được sử dụng cho đăng nhập
        public string Username { get; set; }
        public string Password { get; set; } // *Lưu ý: Luôn mã hóa (hash) mật khẩu trong thực tế!*

        // Khóa ngoại (Foreign Key)
        public int RoleId { get; set; }

        // Navigation Property (Liên kết 1-nhiều với Roles)
        public Role Role { get; set; }
    }
}
