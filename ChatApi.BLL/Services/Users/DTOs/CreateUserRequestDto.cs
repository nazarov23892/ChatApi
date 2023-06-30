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
        [RegularExpression(pattern: "^[a-zA-Z][a-zA-Z0-9]*$", ErrorMessage = "The Name must start with a letter and can only contain letters and numbers.")]
        public string UserName { get; set; } = string.Empty;
    }
}
