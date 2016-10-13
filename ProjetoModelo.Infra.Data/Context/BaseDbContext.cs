using System.Data.Entity;
using Moneta.Infra.Data.Interfaces;

namespace Moneta.Infra.Data.Context
{
    public class BaseDbContext : DbContext, IDbContext
    {
        public BaseDbContext(string nameOrConnectionString) 
            :base(nameOrConnectionString)
        {
        }

        public new IDbSet<T> Set<T>() where T : class
        {
            return base.Set<T>();
        }

        public void SetProxyCreationEnabledToFalse()
        {
            Configuration.ProxyCreationEnabled = false;
        }
    }
}