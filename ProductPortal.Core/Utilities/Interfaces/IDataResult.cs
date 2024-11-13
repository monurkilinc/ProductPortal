using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductPortal.Core.Utilities.Interfaces
{
    public interface IDataResult<T> : IResult
    {
        //Islemin sonucunda dondurulecek veri
        T Data { get; }
    }
}
