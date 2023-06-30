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
        [StringLength(maximumLength: 20, MinimumLength = 3)]
        public string UserName { get; set; } = string.Empty;
    }
}
