using DotNetCore.EnterpriseTemplate.Domain.Repositories;
using DotNetCore.EnterpriseTemplate.ORM;
using DotNetCore.EnterpriseTemplate.ORM.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetCore.EnterpriseTemplate.IoC.ModuleInitializers;

public class InfrastructureModuleInitializer : IModuleInitializer
{
    public void Initialize(WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<DbContext>(provider => provider.GetRequiredService<DefaultContext>());
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddTransient<ISaleRepository, SaleRepository>();
        builder.Services.AddTransient<IBranchRepository, BranchRepository>();
        builder.Services.AddTransient<ICustomerRepository, CustomerRepository>();
        builder.Services.AddTransient<IProductRepository, ProductRepository>();
        builder.Services.AddTransient<ISaleItemRepository, SaleItemRepository>();
    }
}