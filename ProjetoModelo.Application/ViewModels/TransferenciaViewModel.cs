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
            LancamentoOrigem = new LancamentoViewModel(DateTime.Now);
            LancamentoOrigem.Descricao = "Transferência entre contas";
            LancamentoOrigem.TipoDeTransacao = TipoTransacaoEnum.Transferencia;
            LancamentoOrigem.Ativo = true;
            LancamentoOrigem.Pago = false;
            LancamentoOrigem.Fake = false;
        }

        public TransferenciaViewModel(DateTime MesAnoCompetencia)
        {
            LancamentoOrigem = new LancamentoViewModel(MesAnoCompetencia);
            LancamentoOrigem.Descricao = "Transferência entre contas";
            LancamentoOrigem.TipoDeTransacao = TipoTransacaoEnum.Transferencia;
            LancamentoOrigem.Ativo = true;
            LancamentoOrigem.Pago = false;
            LancamentoOrigem.Fake = false;
        }

        [DisplayName("Lançamento Origem")]
        public virtual LancamentoViewModel LancamentoOrigem { get; set; }

        [DisplayName("Conta Destino")]
        public Guid ContaIdDestino { get; set; }
    }
}