using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Application.Branches.GetBranch;
using Ambev.DeveloperEvaluation.Application.Branches.GetBranches;
using Ambev.DeveloperEvaluation.Application.Branches.CreateBranch;
using Ambev.DeveloperEvaluation.WebApi.Features.Branches.CreateBranch;

namespace Ambev.DeveloperEvaluation.WebApi.Mappings;

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