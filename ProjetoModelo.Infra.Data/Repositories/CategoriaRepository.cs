using System.Data.Entity;
using Moneta.Domain.Entities;
using Moneta.Domain.Interfaces.Repository;
using Moneta.Infra.Data.Context;

namespace Moneta.Infra.Data.Repositories
{
    public class CategoriaRepository : RepositoryBase<Categoria, MonetaContext>, ICategoriaRepository
    {
        public override void Remove(Categoria categoria)
        {
            var entry = Context.Entry(categoria);
            DbSet.Attach(categoria);
            entry.State = EntityState.Deleted;
        }
    }
}
