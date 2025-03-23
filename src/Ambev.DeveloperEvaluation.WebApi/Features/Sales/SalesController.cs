using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Ambev.DeveloperEvaluation.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaleController : ControllerBase
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IBranchRepository _branchRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IProductRepository _productRepository;
        private readonly ISaleItemRepository _saleItemRepository;

        public SaleController(ISaleRepository saleRepository, IBranchRepository branchRepository, ICustomerRepository customerRepository, IProductRepository productRepository, ISaleItemRepository saleItemRepository)
        {
            _saleRepository = saleRepository;
            _branchRepository = branchRepository;
            _customerRepository = customerRepository;
            _productRepository = productRepository;
            _saleItemRepository = saleItemRepository;
        }

        // Evento fictício para simular o envio de mensagem para o Message Broker
        private void LogEvent(string eventName, string message)
        {
            // Aqui você pode usar um sistema de log ou simular a publicação do evento.
            Console.WriteLine($"{eventName}: {message}");
        }

        /// <summary>
        /// Cria uma nova venda.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Sale>> CreateAsync(Sale sale, CancellationToken cancellationToken = default)
        {
            // Verificar se já existe uma venda com o mesmo SaleNumber
            var existingSale = await _saleRepository.GetBySaleNumberAsync(sale.SaleNumber, cancellationToken);
            if (existingSale != null)
            {
                return Conflict($"A sale with SaleNumber {sale.SaleNumber} already exists.");
            }

            // Verificar e criar o Branch, se necessário
            var branch = sale.BranchId != null ? await _branchRepository.GetByIdAsync(sale.BranchId, cancellationToken) : null;
            if (branch == null && sale.Branch != null)
            {
                branch = new Branch
                {
                    Name = sale.Branch.Name,
                    Address = sale.Branch.Address,
                    Phone = sale.Branch.Phone,
                    Manager = sale.Branch.Manager
                };
                await _branchRepository.CreateAsync(branch, cancellationToken);
            }

            // Verificar e criar o Customer, se necessário
            var customer = sale.CustomerId != null ? await _customerRepository.GetByIdAsync(sale.CustomerId, cancellationToken) : null;
            if (customer == null && sale.Customer != null)
            {
                customer = new Customer
                {
                    Name = sale.Customer.Name,
                    Email = sale.Customer.Email,
                    Phone = sale.Customer.Phone,
                    Address = sale.Customer.Address
                };
                await _customerRepository.CreateAsync(customer, cancellationToken);
            }

            // Criar a Sale
            var newSale = new Sale
            {
                SaleNumber = sale.SaleNumber,
                SaleDate = DateTime.UtcNow,
                Customer = customer, // Cria um novo ID se o cliente não existir
                Branch = branch, // Cria um novo ID se a filial não existir
                TotalAmount = sale.TotalAmount,
                IsCancelled = sale.IsCancelled
            };

            var createdSale = await _saleRepository.CreateAsync(newSale, cancellationToken);

            // Criar os SalesItems (Itens de venda)
            foreach (var item in sale.Items)
            {
                var product = item.ProductId != null ? await _productRepository.GetByIdAsync(item.ProductId, cancellationToken) : null;
                if (product == null && item.Product != null)
                {
                    product = new Product
                    {
                        Name = item.Product.Name,
                        Description = item.Product.Description,
                        Price = item.Product.Price,
                        SKU = item.Product.SKU
                    };
                    await _productRepository.CreateAsync(product, cancellationToken);
                }

                var saleItem = new SaleItem
                {
                    SaleId = createdSale.Id,
                    Product = product, // Cria um novo ID para o produto, se necessário
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    Discount = item.Discount,
                    TotalPrice = item.TotalPrice
                };

                await _saleItemRepository.CreateAsync(saleItem, cancellationToken);
            }

            // Retorna a venda criada com o status 201 (Created)
            return Created($"/api/sales/{createdSale.Id}", createdSale);
        }


        /// <summary>
        /// Recupera uma venda por ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Sale>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var sale = await _saleRepository.GetByIdAsync(id, cancellationToken);

            if (sale == null)
            {
                return NotFound();
            }

            return Ok(sale);
        }

        /// <summary>
        /// Recupera todas as vendas.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Sale>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var sales = await _saleRepository.GetAllAsync(cancellationToken);
            return Ok(sales);
        }

        /// <summary>
        /// Atualiza os detalhes de uma venda.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<Sale>> UpdateAsync(Guid id, Sale sale, CancellationToken cancellationToken = default)
        {
            if (id != sale.Id)
            {
                return BadRequest("Sale ID mismatch.");
            }

            var existingSale = await _saleRepository.GetByIdAsync(id, cancellationToken);
            if (existingSale == null)
            {
                return NotFound();
            }

            // Atualizar os dados da venda (você pode aplicar mais validações aqui)
            existingSale.SaleNumber = sale.SaleNumber;
            existingSale.SaleDate = sale.SaleDate;
            existingSale.TotalAmount = sale.TotalAmount;

            // Atualiza os itens da venda
            existingSale.Items = sale.Items;

            await _saleRepository.UpdateAsync(existingSale, cancellationToken);

            LogEvent("SaleModified", $"Sale {existingSale.SaleNumber} updated.");
            return Ok(existingSale);
        }

        /// <summary>
        /// Cancela uma venda.
        /// </summary>
        [HttpPut("{id}/cancel")]
        public async Task<ActionResult> CancelAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var sale = await _saleRepository.GetByIdAsync(id, cancellationToken);
            if (sale == null)
            {
                return NotFound();
            }

            sale.CancelSale(); // Método para cancelar a venda
            await _saleRepository.UpdateAsync(sale, cancellationToken);

            LogEvent("SaleCancelled", $"Sale {sale.SaleNumber} cancelled.");
            return NoContent();
        }

        /// <summary>
        /// Cancela um item da venda.
        /// </summary>
        [HttpPut("{saleId}/items/{itemId}/cancel")]
        public async Task<ActionResult> CancelItemAsync(Guid saleId, Guid itemId, CancellationToken cancellationToken = default)
        {
            var sale = await _saleRepository.GetByIdAsync(saleId, cancellationToken);
            if (sale == null)
            {
                return NotFound();
            }

            var item = sale.Items.FirstOrDefault(i => i.Id == itemId);
            if (item == null)
            {
                return NotFound();
            }

            item.Discount = 1.0m; // Cancelando o item (desconto de 100%)
            item.TotalPrice = 0m; // Total de preço igual a 0

            await _saleRepository.UpdateAsync(sale, cancellationToken);

            LogEvent("ItemCancelled", $"Item {itemId} from Sale {sale.SaleNumber} cancelled.");
            return NoContent();
        }

        /// <summary>
        /// Deleta uma venda.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var result = await _saleRepository.DeleteAsync(id, cancellationToken);

            if (!result)
            {
                return NotFound();
            }

            LogEvent("SaleDeleted", $"Sale {id} deleted.");
            return NoContent();
        }
    }
}
