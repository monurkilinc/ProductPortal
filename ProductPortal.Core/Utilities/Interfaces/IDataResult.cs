using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProductPortal.Core.Utilities.Results;

namespace ProductPortal.Core.Utilities.Interfaces
{
    public interface IDataResult<T> : IResult
    {
        T Data { get; }
    }
}
