using MediatR;
using System;

namespace Ambev.DeveloperEvaluation.Application.Sales.DeleteSale
{
    public class DeleteSaleCommand : IRequest<Unit> // Alterado para IRequest<Unit>
    {
        public Guid SaleId { get; set; }
    }
}