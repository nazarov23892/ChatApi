using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApi.BLL.Entities
{
    public class Chat
    {
        [Key]
        [Required]
        public string ChatId { get; set; } = string.Empty;

        [Required]
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public ICollection<User> Users { get; set; } = new List<User>();
        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}
