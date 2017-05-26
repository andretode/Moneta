using MySql.Data.MySqlClient;
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
                return new MySqlConnection(ConfigurationManager.ConnectionStrings["Moneta"].ConnectionString);
            }
        }
    }
}
