using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApi.BLL.Services.Users.DTOs
{
    public class CreateUserRequestDto
    {
        [Required]
        public string UserName { get; set; } = string.Empty;
    }
}
