using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApi.BLL.Basic
{
    public interface IValidatableService
    {
        bool HasValidationProblems { get; }

        Dictionary<string, string[]> ValidationProblems { get; }
    }
}
