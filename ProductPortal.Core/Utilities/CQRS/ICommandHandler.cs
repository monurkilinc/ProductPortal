using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductPortal.Core.Utilities.CQRS
{
    public interface ICommandHandler<TCommand,TResult> where TCommand:ICommand<TResult>
    {
        Task<TResult> Handle(TCommand command);
    }
}
