using MediatR;

namespace ProductPortal.Business.Commands.Product
{
    public class DeleteProductCommand :IRequest<Core.Utilities.Results.IResult>
    {
        public int Id { get; set; }

        public DeleteProductCommand(int id)
        {
            Id = id;
        }
    }
}
