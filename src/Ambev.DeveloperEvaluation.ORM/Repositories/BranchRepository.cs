using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

public class BranchRepository : IBranchRepository
{
    private readonly DefaultContext _context;

    public BranchRepository(DefaultContext context)
    {
        _context = context;
    }

    public async Task<Branch> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Branches.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<List<Branch>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Branches
            .Where(x => x.IsActive)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Branch> CreateAsync(Branch branch, CancellationToken cancellationToken)
    {
        _context.Branches.Add(branch);
        await _context.SaveChangesAsync(cancellationToken);
        return branch;
    }
}
