using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApi.BLL.Services
{
    public class ErrorResponseDto
    {
        public ErrorResponseDto(string? error)
        {
            Error = error ?? string.Empty;
        }

        public string Error { get; set; } = string.Empty;
    }
}
