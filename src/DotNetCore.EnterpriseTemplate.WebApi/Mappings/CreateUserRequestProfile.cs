using DotNetCore.EnterpriseTemplate.Application.Users.CreateUser;
using DotNetCore.EnterpriseTemplate.WebApi.Features.Users.CreateUser;
using AutoMapper;

namespace DotNetCore.EnterpriseTemplate.WebApi.Mappings;

public class CreateUserRequestProfile : Profile
{
    public CreateUserRequestProfile()
    {
        CreateMap<CreateUserRequest, CreateUserCommand>();
    }
}