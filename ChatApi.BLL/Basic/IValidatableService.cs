using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApi.BLL.Basic
{
    public interface IValidatableService
    {
        bool HasValidationProblems { get; }

        IImmutableList<string> ValidationProblems { get; }
    }
}
