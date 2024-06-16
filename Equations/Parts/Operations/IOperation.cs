using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathUtils.Equations.Parts.Operations
{
    public interface IOperation : IPart
    {
        static int Priority { get; }
    }
}
