using System.Data.Entity;
using Moneta.Infra.Data.Interfaces;
using MySql.Data.Entity;

namespace Moneta.Infra.Data.Context
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class BaseDbContext : DbContext, IDbContext
    {
        public BaseDbContext(string nameOrConnectionString) 
            :base(nameOrConnectionString)
        {
            DbConfiguration.SetConfiguration(new MySqlEFConfiguration());
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