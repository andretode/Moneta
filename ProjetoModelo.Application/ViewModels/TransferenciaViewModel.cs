using Moneta.Infra.CrossCutting.Enums;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Moneta.Application.ViewModels
{
    public class TransferenciaViewModel
    {
        public TransferenciaViewModel()
        {
            LancamentoPai = InstanciarLancamento(DateTime.Now);
        }

        public TransferenciaViewModel(DateTime MesAnoCompetencia)
        {
            LancamentoPai = InstanciarLancamento(MesAnoCompetencia);
        }

        private LancamentoViewModel InstanciarLancamento(DateTime MesAnoCompetencia)
        {
            var lancamento = new LancamentoViewModel(MesAnoCompetencia);
            lancamento.Descricao = "Transferência entre contas";
            lancamento.TipoDeTransacao = TipoTransacaoEnum.Transferencia;
            lancamento.Ativo = true;
            lancamento.Pago = false;
            lancamento.Fake = false;

            return lancamento;
        }

        public virtual LancamentoViewModel LancamentoPai { get; set; }

        [DisplayName("Conta Origem")]
        public Guid ContaIdOrigem { get; set; }

        [DisplayName("Conta Destino")]
        public Guid ContaIdDestino { get; set; }

        public ContaEnum ConciliarExtratoCom { get; set; }
    }

    public enum ContaEnum { ORIGEM, DESTINO }
}