using System.ComponentModel.DataAnnotations;

namespace SE07203_F1.Models
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // Navigation Property (Liên kết 1-nhiều với Accounts)
        public ICollection<Account> Accounts { get; set; } = new List<Account>();
    }
}
