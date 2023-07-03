using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApi.BLL.Entities
{
    public class Message
    {
        [Key]
        [Required]
        public string MessageId { get; set; } = string.Empty;

        [Required]
        public string Text { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        [Required]
        public string ChatId { get; set; } = string.Empty;
        public Chat Chat { get; set; }

        [Required]
        public string AuthorId { get; set; } = string.Empty;
        public User User { get; set; }
    }
}
