using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Moneta.Application.Interfaces;
using Moneta.Application.Validation;
using Moneta.Application.ViewModels;
using Moneta.Domain.Entities;
using Moneta.Domain.Interfaces.Services;
using Moneta.Infra.Data.Context;
using Moneta.Domain.ValueObjects;
using Moneta.Infra.CrossCutting.Enums;

namespace Moneta.Application
{
    public class LancamentoAppService : AppServiceBase<MonetaContext>, ILancamentoAppService
    {
        private readonly ILancamentoService _lancamentoService;

        public LancamentoAppService(ILancamentoService LancamentoService)
        {
            _lancamentoService = LancamentoService;
        }

        public LancamentoViewModel GetById(Guid id)
        {
            var lancamentoViewModel = Mapper.Map<Lancamento, LancamentoViewModel>(_lancamentoService.GetById(id));
            AjustarLancamentoParaExibir(lancamentoViewModel);
            return lancamentoViewModel;
        }

        public LancamentoViewModel GetByIdReadOnly(Guid id)
        {
            var lancamentoViewModel = Mapper.Map<Lancamento, LancamentoViewModel>(_lancamentoService.GetByIdReadOnly(id));
            AjustarLancamentoParaExibir(lancamentoViewModel);
            return lancamentoViewModel;
        }

        public ValidationAppResult Add(LancamentoViewModel lancamentoViewModel)
        {
            if (lancamentoViewModel.DataVencimento <= DateTime.Now.Date)
                lancamentoViewModel.Pago = true;

            lancamentoViewModel.Valor = AjustarValorParaSalvar(lancamentoViewModel);

            ValidationResult result = null;
            if (lancamentoViewModel.LancamentoParcelado != null && lancamentoViewModel.LancamentoParcelado.TipoDeRepeticao == TipoRepeticao.Parcelado)
            {
                if (lancamentoViewModel.LancamentoParcelado.Periodicidade != 0 && lancamentoViewModel.LancamentoParcelado.NumeroParcelas > 1)
                {
                    var lancamento = Mapper.Map<LancamentoViewModel, Lancamento>(lancamentoViewModel);
                    result = AdicionarLancamentosParcelados(lancamento);
                }
                else
                {
                    //##############################################################
                    // TENTAR MELHORAR O LOCAL QUE ESTE TRATAMENTO DEVE SER FEITO
                    var vr = new ValidationResult();
                    vr.AdicionarErro(new ValidationError("A periodicidade e n° de parcelas são obrigatórios em caso de parcelamento."));
                    return DomainToApplicationResult(vr);
                }
            }
            else if (lancamentoViewModel.LancamentoParcelado != null && lancamentoViewModel.LancamentoParcelado.TipoDeRepeticao == TipoRepeticao.Fixo)
            {
                if (lancamentoViewModel.LancamentoParcelado.Periodicidade != 0)
                {
                    lancamentoViewModel.LancamentoParcelado.LancamentoBaseId = lancamentoViewModel.LancamentoId;
                    lancamentoViewModel.BaseDaSerie = true;

                    var lancamento = Mapper.Map<LancamentoViewModel, Lancamento>(lancamentoViewModel);

                    BeginTransaction();
                    result = _lancamentoService.Adicionar(lancamento);
                    if (!result.IsValid)
                        return DomainToApplicationResult(result);
                    Commit();
                }
                else
                {
                    //##############################################################
                    // TENTAR MELHORAR O LOCAL QUE ESTE TRATAMENTO DEVE SER FEITO
                    var vr = new ValidationResult();
                    vr.AdicionarErro(new ValidationError("A periodicidade é obrigatórios em caso de lançamento fixo."));
                    return DomainToApplicationResult(vr);
                }
            }
            else {
                lancamentoViewModel.LancamentoParcelado = null;
                var lancamento = Mapper.Map<LancamentoViewModel, Lancamento>(lancamentoViewModel);

                BeginTransaction();
                result = _lancamentoService.Adicionar(lancamento);
                if (!result.IsValid)
                    return DomainToApplicationResult(result);
                Commit();
            }

            return DomainToApplicationResult(result);
        }

        private ValidationResult AdicionarLancamentosParcelados(Lancamento lancamento)
        {
            ValidationResult result = null;
            for (int i = 0; i < lancamento.LancamentoParcelado.NumeroParcelas; i++)
            {
                Lancamento novoLancamento = null;

                //O primeiro lançamento precisa ser associado ao LancamentoParcelado para criá-lo em BD
                if (i == 0)
                {
                    novoLancamento = lancamento.Clone(lancamento.DataVencimento);
                    novoLancamento.LancamentoParcelado = lancamento.LancamentoParcelado;
                    novoLancamento.LancamentoParcelado.LancamentoBaseId = lancamento.LancamentoId;
                }
                else
                {
                    switch (lancamento.LancamentoParcelado.Periodicidade)
                    {
                        case (int)PeriodicidadeEnum.Semanal:
                            novoLancamento = lancamento.Clone(lancamento.DataVencimento.AddDays(i * 7));
                            break;
                        case (int)PeriodicidadeEnum.Mensal:
                            novoLancamento = lancamento.Clone(lancamento.DataVencimento.AddMonths(i));
                            break;
                    }
                }

                BeginTransaction();
                result = _lancamentoService.Adicionar(novoLancamento);
                if (!result.IsValid)
                    return result;
                Commit();
            }

            return result;
        }

        public ValidationAppResult AddTransferencia(TransferenciaViewModel transferencia)
        {
            var lancamentoOrigemVM = transferencia.LancamentoOrigem;
            lancamentoOrigemVM.Conta = null;
            var lancamentoOrigem = Mapper.Map<LancamentoViewModel, Lancamento>(lancamentoOrigemVM);
            var lancamentoDestino = lancamentoOrigem.CreateLancamentoTransferenciaPar(transferencia.ContaIdDestino);

            BeginTransaction();

            var result = _lancamentoService.Adicionar(lancamentoOrigem);
            if (!result.IsValid)
                return DomainToApplicationResult(result);
            result = _lancamentoService.Adicionar(lancamentoDestino);
            if (!result.IsValid)
                return DomainToApplicationResult(result);

            Commit();

            //AtualizaTransferenciaParDepoisDeCriado(lancamentoOrigem, lancamentoDestino);
            
            return DomainToApplicationResult(result);
        }

        private void AtualizaTransferenciaParDepoisDeCriado(Lancamento lancamentoOrigem, Lancamento lancamentoDestino)
        {
            lancamentoOrigem = _lancamentoService.GetByIdReadOnly(lancamentoOrigem.LancamentoId);
            lancamentoDestino = _lancamentoService.GetByIdReadOnly(lancamentoDestino.LancamentoId);
            lancamentoOrigem.LancamentoIdTransferencia = lancamentoDestino.LancamentoId;
            lancamentoDestino.LancamentoIdTransferencia = lancamentoOrigem.LancamentoId;

            BeginTransaction();
            _lancamentoService.Update(lancamentoOrigem);
            _lancamentoService.Update(lancamentoDestino);
            Commit();
        }
        
        public void Update(LancamentoViewModel lancamentoViewModel)
        {
            lancamentoViewModel.LancamentoParcelado = null;
            lancamentoViewModel.Valor = AjustarValorParaSalvar(lancamentoViewModel);
            var lancamento = Mapper.Map<LancamentoViewModel, Lancamento>(lancamentoViewModel);

            BeginTransaction();
            _lancamentoService.Update(lancamento);
            Commit();
        }

        public void UpdateEmSerie(LancamentoViewModel lancamentoViewModel)
        {
            //lancamentoViewModel.LancamentoParcelado = null;
            lancamentoViewModel.Valor = AjustarValorParaSalvar(lancamentoViewModel);
            var lancamento = Mapper.Map<LancamentoViewModel, Lancamento>(lancamentoViewModel);

            BeginTransaction();
            _lancamentoService.UpdateEmSerie(lancamento);
            Commit();
        }

        public void RemoveEmSerie(LancamentoViewModel lancamentoViewModel)
        {
            lancamentoViewModel.Valor = AjustarValorParaSalvar(lancamentoViewModel);
            var lancamento = Mapper.Map<LancamentoViewModel, Lancamento>(lancamentoViewModel);

            BeginTransaction();
            _lancamentoService.RemoveEmSerie(lancamento);
            Commit();
        }

        public void Remove(LancamentoViewModel LancamentoViewModel)
        {
            var lancamento = Mapper.Map<LancamentoViewModel, Lancamento>(LancamentoViewModel);

            BeginTransaction();
            _lancamentoService.Remove(lancamento);
            Commit();
        }

        public void Dispose()
        {
            _lancamentoService.Dispose();
        }

        public List<Tuple<DateTime, decimal, decimal, decimal>> GetSaldoDoMesPorDia(LancamentosDoMesViewModel lancamentosDoMesViewModel, bool resumido)
        {
            var lancamentosDoMes = Mapper.Map<LancamentosDoMesViewModel, AgregadoLancamentosDoMes>(lancamentosDoMesViewModel);
            return _lancamentoService.GetSaldoDoMesPorDia(lancamentosDoMes, resumido);
        }

        public LancamentosDoMesViewModel GetLancamentosDoMes(LancamentosDoMesViewModel lancamentosDoMesViewModel)
        {
            AgregadoLancamentosDoMes lancamentosDoMes = Mapper.Map<LancamentosDoMesViewModel, AgregadoLancamentosDoMes>(lancamentosDoMesViewModel);
            return Mapper.Map<AgregadoLancamentosDoMes, LancamentosDoMesViewModel>(_lancamentoService.GetLancamentosDoMes(lancamentosDoMes));
        }

        public void AjustarLancamentoParaExibir(LancamentoViewModel lancamentoViewModel)
        {
            if(lancamentoViewModel.TipoDeTransacao == 0)
            {
                if (lancamentoViewModel.Valor > 0)
                {
                    lancamentoViewModel.TipoDeTransacao = TipoTransacaoEnum.Receita;
                }
                else
                {
                    lancamentoViewModel.TipoDeTransacao = TipoTransacaoEnum.Despesa;
                    lancamentoViewModel.Valor = lancamentoViewModel.Valor * -1;
                }
            }
            else if (lancamentoViewModel.TipoDeTransacao == TipoTransacaoEnum.Despesa)
            {
                lancamentoViewModel.Valor = Math.Abs(lancamentoViewModel.Valor);
            }
        }

        #region Metodos privados
        private decimal AjustarValorParaSalvar(LancamentoViewModel lancamentoViewModel)
        {
            decimal valorAjustado;
            if (lancamentoViewModel.TipoDeTransacao == TipoTransacaoEnum.Despesa && Math.Sign(lancamentoViewModel.Valor) == 1)
                valorAjustado = lancamentoViewModel.Valor * -1;
            else if (lancamentoViewModel.TipoDeTransacao == TipoTransacaoEnum.Receita && Math.Sign(lancamentoViewModel.Valor) == -1)
                valorAjustado = lancamentoViewModel.Valor * -1;
            else
                valorAjustado = lancamentoViewModel.Valor;

            return valorAjustado;
        }
        #endregion
    }
}
