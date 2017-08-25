using Moneta.Domain.Entities;
using Moneta.Domain.Specification.Categorias;
using Moneta.Domain.Specification.Lancamentos;
using Moneta.Domain.Validation.Base;
using Moneta.Domain.ValueObjects;

namespace Moneta.Domain.Validation.Lancamentos
{
    public class AgregadoLancamentosDoMesEstaAptoParaConsulta : FiscalBase<AgregadoLancamentosDoMes>
    {
        public AgregadoLancamentosDoMesEstaAptoParaConsulta()
        {
            var possuiIdAppUser = new LancamentoPossuiIdAppUser();

            base.AdicionarRegra("PossuiIdUsuario", new Regra<AgregadoLancamentosDoMes>(possuiIdAppUser, "LancamentoDoMes não possui ID do Usuario associado"));
        }
    }
}
