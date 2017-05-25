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
        private readonly ILancamentoConciliacaoService _lancamentoConciliacaoService;
        private readonly ILancamentoParceladoAppService _lancamentoParceladoServiceApp;

        public LancamentoAppService(ILancamentoService LancamentoService,
            ILancamentoConciliacaoService lancamentoConciliacaoService,
            ILancamentoParceladoAppService lancamentoParceladoServiceApp)
        {
            _lancamentoService = LancamentoService;
            _lancamentoConciliacaoService = lancamentoConciliacaoService;
            _lancamentoParceladoServiceApp = lancamentoParceladoServiceApp;
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

            if (lancamentoViewModel.LancamentoParcelado != null && lancamentoViewModel.LancamentoParcelado.TipoDeRepeticao == TipoRepeticao.Parcelado)
            {
                return TratarLancamentoParcelado(lancamentoViewModel);
            }
            else if (lancamentoViewModel.LancamentoParcelado != null && lancamentoViewModel.LancamentoParcelado.TipoDeRepeticao == TipoRepeticao.Fixo)
            {
                return TratarLancamentoFixo(lancamentoViewModel);
            }
            else {
                return TratarLancamentoUnico(lancamentoViewModel);
            }
        }
        
        public ValidationAppResult AddTransferencia(TransferenciaViewModel transferencia)
        {
            var lancamentoOrigemVM = transferencia.LancamentoOrigem;
            lancamentoOrigemVM.Conta = null;

            //Para garantir que o valor negativo vindo do lançamento vindo do extrato seja ajustado para gerar origem negativa mais a frente no processamento
            lancamentoOrigemVM.Valor = Math.Abs(lancamentoOrigemVM.Valor);
            
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

        private void RemoveEmSerie(LancamentoViewModel lancamentoViewModel)
        {
            lancamentoViewModel.Valor = AjustarValorParaSalvar(lancamentoViewModel);
            var lancamento = Mapper.Map<LancamentoViewModel, Lancamento>(lancamentoViewModel);
            _lancamentoService.RemoveEmSerie(lancamento);
        }

        public void RemoveTransferencia(LancamentoViewModel lancamentoVM)
        {
            var lancamento = Mapper.Map<LancamentoViewModel, Lancamento>(lancamentoVM);

            BeginTransaction();
            _lancamentoService.RemoveTransferencia(lancamento);
            Commit();
        }

        public void Remove(LancamentoViewModel LancamentoViewModel)
        {
            
            BeginTransaction();

            if (LancamentoViewModel.LancamentoParcelado == null ||
                LancamentoViewModel.LancamentoParcelado.TipoDeAlteracaoDaRepeticao ==
                    TipoDeAlteracaoDaRepeticaoEnum.AlterarApenasEste)
            {
                LancamentoViewModel.Ativo = false;
                if (LancamentoViewModel.Fake)
                    Add(LancamentoViewModel);
                else
                {
                    var lancamento = Mapper.Map<LancamentoViewModel, Lancamento>(LancamentoViewModel);
                    _lancamentoService.Remove(lancamento);
                }
            }
            else
            {
                var tipoDeAlteracaoDaRepeticao = LancamentoViewModel.LancamentoParcelado.TipoDeAlteracaoDaRepeticao;
                LancamentoViewModel.LancamentoParcelado = _lancamentoParceladoServiceApp
                    .GetByIdReadOnly((Guid)LancamentoViewModel.LancamentoParceladoId);
                LancamentoViewModel.LancamentoParcelado.TipoDeAlteracaoDaRepeticao = tipoDeAlteracaoDaRepeticao;
                RemoveEmSerie(LancamentoViewModel);
            }

            Commit();
        }

        public void RemoveAll(IEnumerable<LancamentoViewModel> lancamentos)
        {
            BeginTransaction();
            foreach (var l in lancamentos)
                Remove(l);
            Commit();
        }

        public void Dispose()
        {
            _lancamentoService.Dispose();
        }

        public void TrocarPago(IEnumerable<LancamentoViewModel> lancamentosViewModel)
        {
            var lancamentos = Mapper.Map<IEnumerable<LancamentoViewModel>, IEnumerable<Lancamento>>(lancamentosViewModel);

            BeginTransaction();
            _lancamentoService.TrocarPago(lancamentos);
            Commit();
        }

        public int ImportarOfxParaGrupoDeLancamento(string caminhoOfx, Guid contaId, DateTime mesAnoCompetencia, Guid grupoLancamentoId)
        {
            BeginTransaction();
            var quantidade = _lancamentoService.ImportarOfxParaGrupoDeLancamento(caminhoOfx, contaId, mesAnoCompetencia, grupoLancamentoId);
            Commit();

            return quantidade;
        }

        public List<Tuple<DateTime, decimal, decimal, decimal>> GetSaldoDoMesPorDia(LancamentosDoMesViewModel lancamentosDoMesViewModel, bool resumido)
        {
            var lancamentosDoMes = Mapper.Map<LancamentosDoMesViewModel, AgregadoLancamentosDoMes>(lancamentosDoMesViewModel);
            return _lancamentoService.GetSaldoDoMesPorDia(lancamentosDoMes, resumido);
        }

        public GraficoSaldoPorCategoriaViewModel GetDespesasPorCategoria(Guid ContaIdFiltro, DateTime mesAnoCompetencia, bool pago)
        {
            return new GraficoSaldoPorCategoriaViewModel(_lancamentoService.GetDespesasPorCategoria(ContaIdFiltro, mesAnoCompetencia, pago));
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

        public IEnumerable<LancamentoAgrupadoViewModel> GetLancamentosSugeridosParaConciliacao(ExtratoBancarioViewModel extrato)
        {
            var extratoVM = Mapper.Map<ExtratoBancarioViewModel, ExtratoBancario>(extrato);
            return Mapper.Map<IEnumerable<LancamentoAgrupado>, IEnumerable<LancamentoAgrupadoViewModel>>(_lancamentoConciliacaoService.GetLancamentosSugeridosParaConciliacao(extratoVM));
        }

        #region Metodos privados


        private ValidationAppResult TratarLancamentoParcelado(LancamentoViewModel lancamentoViewModel)
        {
            if (lancamentoViewModel.LancamentoParcelado.Periodicidade != 0 && lancamentoViewModel.LancamentoParcelado.NumeroParcelas > 1)
            {
                var lancamento = Mapper.Map<LancamentoViewModel, Lancamento>(lancamentoViewModel);
                return DomainToApplicationResult(AdicionarLancamentosParcelados(lancamento));
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

        private ValidationAppResult TratarLancamentoFixo(LancamentoViewModel lancamentoViewModel)
        {
            ValidationResult result = null;
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
                vr.AdicionarErro(new ValidationError("A periodicidade é obrigatória em caso de lançamento fixo."));
                return DomainToApplicationResult(vr);
            }

            return DomainToApplicationResult(result);
        }

        private ValidationAppResult TratarLancamentoUnico(LancamentoViewModel lancamentoViewModel)
        {
            lancamentoViewModel.LancamentoParcelado = null;
            var lancamento = Mapper.Map<LancamentoViewModel, Lancamento>(lancamentoViewModel);

            BeginTransaction();
            var result = _lancamentoService.Adicionar(lancamento);
            if (!result.IsValid)
                return DomainToApplicationResult(result);
            Commit();

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
                    lancamento.LancamentoParcelado.LancamentoBaseId = novoLancamento.LancamentoId;
                    novoLancamento.LancamentoParcelado = lancamento.LancamentoParcelado;
                }
                else
                {
                    switch (lancamento.LancamentoParcelado.Periodicidade)
                    {
                        case (int)PeriodicidadeEnum.Diario:
                            novoLancamento = lancamento.Clone(lancamento.DataVencimento.AddDays(i));
                            break;
                        case (int)PeriodicidadeEnum.Semanal:
                            novoLancamento = lancamento.Clone(lancamento.DataVencimento.AddDays(i * (int)PeriodicidadeEnum.Semanal));
                            break;
                        case (int)PeriodicidadeEnum.Quinzenal:
                            novoLancamento = lancamento.Clone(lancamento.DataVencimento.AddDays(i * (int)PeriodicidadeEnum.Quinzenal));
                            break;
                        case (int)PeriodicidadeEnum.Mensal:
                            novoLancamento = lancamento.Clone(lancamento.DataVencimento.AddMonths(i));
                            break;
                        case (int)PeriodicidadeEnum.Trimestral:
                            novoLancamento = lancamento.Clone(lancamento.DataVencimento.AddMonths(i * 3));
                            break;
                        case (int)PeriodicidadeEnum.Semestral:
                            novoLancamento = lancamento.Clone(lancamento.DataVencimento.AddMonths(i * 6));
                            break;
                        case (int)PeriodicidadeEnum.Anual:
                            novoLancamento = lancamento.Clone(lancamento.DataVencimento.AddYears(i));
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
