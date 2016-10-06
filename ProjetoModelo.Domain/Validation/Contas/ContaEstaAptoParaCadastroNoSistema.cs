using Moneta.Domain.Entities;
using Moneta.Domain.Specification.Contas;
using Moneta.Domain.Validation.Base;

namespace Moneta.Domain.Validation.Contas
{
    public class ContaEstaAptoParaCadastroNoSistema : FiscalBase<Conta>
    {
        public ContaEstaAptoParaCadastroNoSistema()
        {
            var contaAtivo = new ContaPossuiStatusAtivo();

            base.AdicionarRegra("ContaAtiva", new Regra<Conta>(contaAtivo, "Conta não está ativa no sistema"));
        }
    }
}
