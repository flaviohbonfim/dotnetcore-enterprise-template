using AutoMapper;
using DotNetCore.EnterpriseTemplate.Application.Users.GetUser;
namespace DotNetCore.EnterpriseTemplate.WebApi.Features.Users.GetUser;

/// <summary>
/// Profile for mapping GetUser feature requests to commands
/// </summary>
public class GetUserProfile : Profile
{
    /// <summary>
    /// Initializes the mappings for GetUser feature
    /// </summary>
    public GetUserProfile()
    {
        CreateMap<Guid, GetUserCommand>()
            .ConstructUsing(id => new GetUserCommand(id));
            
        CreateMap<GetUserResult, GetUserResponse>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role));
    }
}
