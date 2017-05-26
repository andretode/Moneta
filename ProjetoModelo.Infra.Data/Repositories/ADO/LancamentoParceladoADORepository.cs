using Moneta.Domain.Interfaces.Repository.ADO;
using Moneta.Infra.Data.Repositories.ADO;
using System;
using System.Data;
using System.Linq;
using Dapper;

namespace Moneta.Infra.Data.Repositories
{
    public class LancamentoParceladoADORepository : BaseADORepository, ILancamentoParceladoADORepository
    {
        public void ClearAll(Guid lancamentoParceladoId)
        {
            using (IDbConnection cn = Connection)
            {
                cn.Open();

                var sqlLancamentosParcelados = @"DELETE FROM Lancamento
                    WHERE LancamentoParceladoId = '{0}'";
                sqlLancamentosParcelados = String.Format(sqlLancamentosParcelados, lancamentoParceladoId);
                cn.ExecuteScalar(sqlLancamentosParcelados);

                var sqlLancamentoParcelado = @"DELETE FROM LancamentoParcelado
                    WHERE LancamentoParceladoId = '{0}'";
                sqlLancamentoParcelado = String.Format(sqlLancamentoParcelado, lancamentoParceladoId);

                cn.ExecuteScalar(sqlLancamentoParcelado);
            }
        }

        public void ForceRemove(Guid? lancamentoParceladoId)
        {
            using (IDbConnection cn = Connection)
            {
                cn.Open();

                var sqlLancamentosParcelados = @"DELETE FROM Lancamento
                    WHERE LancamentoParceladoId = '{0}'";
                sqlLancamentosParcelados = String.Format(sqlLancamentosParcelados, lancamentoParceladoId);

                cn.ExecuteScalar(sqlLancamentosParcelados);
            }
        }
    }
}
