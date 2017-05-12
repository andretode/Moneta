using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moneta.Domain.Entities;
using Moneta.Domain.Interfaces.Repository.ADO;

namespace Moneta.Infra.Data.Repositories.ADO
{
    public class LancamentoADORepository : BaseADORepository
    {
        public void DeleteAllLancamentosAndLancamentosParceladosSemBase(Guid? lancamentoParceladoId)
        {
            if(lancamentoParceladoId != null)
            {
                DateTime tempoExecucao = DateTime.Now;

                var sqlTabelaTemporaria =
                    "CREATE TEMPORARY TABLE IF NOT EXISTS moneta.LancamentoParceladoSemBase AS (" +
                    "   SELECT LancamentoParceladoId FROM moneta.LancamentoParcelado" +
                    "   WHERE LancamentoBaseId NOT IN (" +
                    "       SELECT LancamentoId FROM moneta.Lancamento" +
                    "           WHERE LancamentoParceladoId='{0}')"+
                    "   AND LancamentoParceladoId='{0}')";
                sqlTabelaTemporaria = String.Format(sqlTabelaTemporaria, lancamentoParceladoId);

                var sqlDeleteLancamentos =
                    "DELETE FROM Lancamento" +
                    "   WHERE LancamentoParceladoId IN (" +
                    "       SELECT LancamentoParceladoId FROM LancamentoParceladoSemBase)";

                var sqlDelteLancamentosParcelados =
                    "DELETE FROM LancamentoParcelado" +
                    "   WHERE LancamentoParceladoId IN (" +
                    "       SELECT LancamentoParceladoId FROM LancamentoParceladoSemBase)";

                var sqlDropTabelaTemporaria =
                    "DROP TABLE LancamentoParceladoSemBase";

                Connection.ExecuteScalar(CommandType.Text, sqlTabelaTemporaria);
                Connection.ExecuteScalar(CommandType.Text, sqlDeleteLancamentos);
                Connection.ExecuteScalar(CommandType.Text, sqlDelteLancamentosParcelados);
                Connection.ExecuteScalar(CommandType.Text, sqlDropTabelaTemporaria);

                System.Diagnostics.Debug.Print("DEBUG: Tempo de remoção dos lançamentos parcelados sem base: "
                    + DateTime.Now.Subtract(tempoExecucao).ToString());
            }
            else
            {
                System.Diagnostics.Debug.Print("DEBUG: Não tentou remover lançamentos parcelados sem base");
            }
        }
    }
}
