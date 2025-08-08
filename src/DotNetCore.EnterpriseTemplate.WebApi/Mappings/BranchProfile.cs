using AutoMapper;
using DotNetCore.EnterpriseTemplate.Domain.Entities;
using DotNetCore.EnterpriseTemplate.Application.Branches.GetBranch;
using DotNetCore.EnterpriseTemplate.Application.Branches.GetBranches;
using DotNetCore.EnterpriseTemplate.Application.Branches.CreateBranch;
using DotNetCore.EnterpriseTemplate.WebApi.Features.Branches.CreateBranch;

namespace DotNetCore.EnterpriseTemplate.WebApi.Mappings;

public class BranchProfile : Profile
{
    public BranchProfile()
    {
        CreateMap<Branch, GetBranchResult>();
        CreateMap<Branch, GetBranchesResult>();
        CreateMap<CreateBranchRequest, CreateBranchCommand>();
        CreateMap<Branch, CreateBranchResult>();
    }
}