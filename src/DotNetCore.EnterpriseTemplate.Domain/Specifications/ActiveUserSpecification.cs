using DotNetCore.EnterpriseTemplate.Domain.Entities;
using DotNetCore.EnterpriseTemplate.Domain.Enums;

namespace DotNetCore.EnterpriseTemplate.Domain.Specifications;

public class ActiveUserSpecification : ISpecification<User>
{
    public bool IsSatisfiedBy(User user)
    {
        return user.Status == UserStatus.Active;
    }
}
