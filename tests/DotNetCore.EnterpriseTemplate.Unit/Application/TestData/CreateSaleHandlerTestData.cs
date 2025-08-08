using DotNetCore.EnterpriseTemplate.Application.Sales.CreateSale;
using Bogus;

namespace DotNetCore.EnterpriseTemplate.Unit.Domain;

public static class CreateSaleHandlerTestData
{
    private static readonly Faker<CreateSaleCommand> createSaleCommandFaker = new Faker<CreateSaleCommand>()
        .RuleFor(s => s.CustomerId, f => Guid.NewGuid())
        .RuleFor(s => s.BranchId, f => Guid.NewGuid())
        .RuleFor(s => s.SaleNumber, f => f.Random.AlphaNumeric(10))
        .RuleFor(s => s.SaleDate, f => f.Date.Recent())
        .RuleFor(s => s.Items, f => GenerateSaleItems(f.Random.Number(1, 5)).ToList());

    private static readonly Faker<CreateSaleItemCommand> saleItemCommandFaker = new Faker<CreateSaleItemCommand>()
        .RuleFor(i => i.ProductId, f => Guid.NewGuid())
        .RuleFor(i => i.Quantity, f => f.Random.Number(1, 20))
        .RuleFor(i => i.UnitPrice, f => decimal.Parse(f.Commerce.Price()))
        .RuleFor(i => i.Discount, f => 0m)
        .RuleFor(i => i.TotalPrice, (f, i) => i.Quantity * i.UnitPrice - i.Discount);

    private static IEnumerable<CreateSaleItemCommand> GenerateSaleItems(int count)
    {
        return saleItemCommandFaker.Generate(count);
    }

    public static CreateSaleCommand GenerateValidCommand()
    {
        var command = createSaleCommandFaker.Generate();
        command.TotalAmount = command.Items.Sum(i => i.TotalPrice);
        return command;
    }
}