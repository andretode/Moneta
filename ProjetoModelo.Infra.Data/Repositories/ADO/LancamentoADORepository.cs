using System;
using System.Data;
using Dapper;

namespace Moneta.Infra.Data.Repositories.ADO
{
    public class LancamentoADORepository : BaseADORepository
    {
        public void ForceRemove(Guid id)
        {
            using (IDbConnection cn = Connection)
            {
                cn.Open();

                var sql = @"DELETE FROM Lancamento WHERE LancamentoId = '{0}'";
                sql = String.Format(sql, id);

                cn.ExecuteScalar(sql);
            }
        }
    }
}
