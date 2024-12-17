using MediatR;

namespace ProductPortal.Business.Commands.Product
{
    public class UpdateProductCommand:IRequest<Core.Utilities.Results.IResult>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public int Stock { get; set; }
        public decimal Price { get; set; }
        public string? ImageURL { get; set; }
    }
}
