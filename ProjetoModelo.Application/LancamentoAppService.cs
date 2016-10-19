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

        public ValidationAppResult Add(LancamentoViewModel lancamentoViewModel)
        {
            if (lancamentoViewModel.DataVencimento <= DateTime.Now.Date)
                lancamentoViewModel.Pago = true;

            lancamentoViewModel.Valor = AjustarValorParaSalvar(lancamentoViewModel);

            if (lancamentoViewModel.LancamentoParcelado != null && lancamentoViewModel.LancamentoParcelado.Periodicidade != 0)
                lancamentoViewModel.LancamentoParcelado.DataInicio = lancamentoViewModel.DataVencimento;
            else
                lancamentoViewModel.LancamentoParcelado = null;

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

                    //tratar a adição em BD do lançamento fake para setar suas entidades relacionadas como null para ao adicionar em BD nao dar erro tentado cadastra-las
                    //lancamentoViewModel.Categoria = null;
                    //lancamentoViewModel.Conta = null;

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
                var novoLancamento = lancamento.CloneFake();

                if (i == 0)
                    novoLancamento.LancamentoParcelado = lancamento.LancamentoParcelado;

                novoLancamento.Descricao += " (" + (i + 1) + "/" + lancamento.LancamentoParcelado.NumeroParcelas + ")";
                switch(lancamento.LancamentoParcelado.Periodicidade)
                {
                    case (int)PeriodicidadeEnum.Semanal:
                        novoLancamento.DataVencimento = novoLancamento.DataVencimento.AddDays(i * 7);
                        break;
                    case (int)PeriodicidadeEnum.Mensal:
                        novoLancamento.DataVencimento = novoLancamento.DataVencimento.AddMonths(i);
                        break;
                }

                BeginTransaction();
                result = _lancamentoService.Adicionar(novoLancamento);
                if (!result.IsValid)
                    return result;
                Commit();
            }

            return result;
        }

        public LancamentoViewModel GetById(Guid id)
        {
            var lancamentoVM = Mapper.Map<Lancamento, LancamentoViewModel>(_lancamentoService.GetById(id));
            lancamentoVM = AjustarLancamentoParaExibir(lancamentoVM);
            return lancamentoVM;
        }

        public LancamentoViewModel GetByIdReadOnly(Guid id)
        {
            var lancamentoVM = Mapper.Map<Lancamento, LancamentoViewModel>(_lancamentoService.GetByIdReadOnly(id));

            if (lancamentoVM != null)
                lancamentoVM = AjustarLancamentoParaExibir(lancamentoVM);

            return lancamentoVM;
        }

        public IEnumerable<LancamentoViewModel> GetAll()
        {
            return Mapper.Map<IEnumerable<Lancamento>, IEnumerable<LancamentoViewModel>>(_lancamentoService.GetAll());
        }

        public IEnumerable<LancamentoViewModel> GetAllReadOnly()
        {
            return Mapper.Map<IEnumerable<Lancamento>, IEnumerable<LancamentoViewModel>>(_lancamentoService.GetAllReadOnly());
        }

        public void Update(LancamentoViewModel lancamentoViewModel)
        {
            lancamentoViewModel.Valor = AjustarValorParaSalvar(lancamentoViewModel);
            var Lancamento = Mapper.Map<LancamentoViewModel, Lancamento>(lancamentoViewModel);

            BeginTransaction();
            _lancamentoService.Update(Lancamento);
            Commit();
        }

        public void Remove(LancamentoViewModel LancamentoViewModel)
        {
            var Lancamento = Mapper.Map<LancamentoViewModel, Lancamento>(LancamentoViewModel);

            BeginTransaction();
            _lancamentoService.Remove(Lancamento);
            Commit();
        }

        public void Dispose()
        {
            _lancamentoService.Dispose();
        }

        public LancamentosDoMesViewModel GetLancamentosDoMes(LancamentosDoMesViewModel lancamentosDoMesViewModel)
        {
            AgregadoLancamentosDoMes lancamentosDoMes = Mapper.Map<LancamentosDoMesViewModel, AgregadoLancamentosDoMes>(lancamentosDoMesViewModel);
            return Mapper.Map<AgregadoLancamentosDoMes, LancamentosDoMesViewModel>(_lancamentoService.GetLancamentosDoMes(lancamentosDoMes));
        }

        #region Metodos privados
        private decimal AjustarValorParaSalvar(LancamentoViewModel lancamentoViewModel)
        {
            decimal valorAjustado;
            if (lancamentoViewModel.Transacao == TipoTransacaoEnum.Despesa && Math.Sign(lancamentoViewModel.Valor) == 1)
                valorAjustado = lancamentoViewModel.Valor * -1;
            else if (lancamentoViewModel.Transacao == TipoTransacaoEnum.Receita && Math.Sign(lancamentoViewModel.Valor) == -1)
                valorAjustado = lancamentoViewModel.Valor * -1;
            else
                valorAjustado = lancamentoViewModel.Valor;

            return valorAjustado;
        }
        private LancamentoViewModel AjustarLancamentoParaExibir(LancamentoViewModel lancamentoViewModel)
        {
            if (lancamentoViewModel.Valor > 0)
            {
                lancamentoViewModel.Transacao = TipoTransacaoEnum.Receita;
            }
            else
            {
                lancamentoViewModel.Transacao = TipoTransacaoEnum.Despesa;
                lancamentoViewModel.Valor = lancamentoViewModel.Valor * -1;
            }

            return lancamentoViewModel;
        }
        #endregion
    }
}
