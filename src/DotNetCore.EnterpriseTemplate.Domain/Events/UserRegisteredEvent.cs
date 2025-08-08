using DotNetCore.EnterpriseTemplate.Domain.Entities;

namespace DotNetCore.EnterpriseTemplate.Domain.Events
{
    public class UserRegisteredEvent
    {
        public User User { get; }

        public UserRegisteredEvent(User user)
        {
            User = user;
        }
    }
}
