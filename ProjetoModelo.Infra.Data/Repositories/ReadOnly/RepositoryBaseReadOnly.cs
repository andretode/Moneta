
using Devart.Data.PostgreSql;
using System.Configuration;
using System.Data;

namespace Moneta.Infra.Data.Repositories.ReadOnly
{
    public class RepositoryBaseReadOnly
    {
        public IDbConnection Connection
        {
            get
            {
                return new PgSqlConnection(ConfigurationManager.ConnectionStrings["Moneta"].ConnectionString);
            }
        }
    }
}
