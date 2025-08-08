using Microsoft.AspNetCore.Builder;

namespace DotNetCore.EnterpriseTemplate.IoC;

public interface IModuleInitializer
{
    void Initialize(WebApplicationBuilder builder);
}
