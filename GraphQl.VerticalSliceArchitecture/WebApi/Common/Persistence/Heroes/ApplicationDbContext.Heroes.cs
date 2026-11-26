using Domain.Heroes;

// Preserva o namespace em classes parciais
// ReSharper disable once CheckNamespace
namespace WebApi.Common.Persistence;

public partial class ApplicationDbContext
{
	public DbSet<Hero> Heroes => AggregateRootSet<Hero>();
}