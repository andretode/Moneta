using System.Data.Entity;
using Moneta.Domain.Entities;
using Moneta.Domain.Interfaces.Repository;
using Moneta.Infra.Data.Context;

namespace Moneta.Infra.Data.Repositories
{
    public class ExtratoBancarioRepository : RepositoryBase<ExtratoBancario, MonetaContext>, IExtratoBancarioRepository
    {
        public override void Remove(ExtratoBancario extratoBancario)
        {
            var entry = Context.Entry(extratoBancario);
            DbSet.Attach(extratoBancario);
            entry.State = EntityState.Deleted;
        }
    }
}
