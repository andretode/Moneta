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
            if (lancamentoViewModel.DataVencimento < DateTime.Now.Date)
                lancamentoViewModel.Pago = true;

            lancamentoViewModel.Valor = AjustarValorParaSalvar(lancamentoViewModel);
            var Lancamento = Mapper.Map<LancamentoViewModel, Lancamento>(lancamentoViewModel);

            BeginTransaction();

            var result = _lancamentoService.Adicionar(Lancamento);
            if (!result.IsValid)
                return DomainToApplicationResult(result);
            
            Commit();

            return DomainToApplicationResult(result);
        }

        public LancamentoViewModel GetById(Guid id)
        {
            var lancamentoVM = Mapper.Map<Lancamento, LancamentoViewModel>(_lancamentoService.GetById(id));
            lancamentoVM = AjustarLancamentoParaExibir(lancamentoVM);
            return lancamentoVM;
        }

        public LancamentoViewModel GetByIdReadOnly(Guid id)
        {
            return Mapper.Map<Lancamento, LancamentoViewModel>(_lancamentoService.GetByIdReadOnly(id));
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

        public LancamentosDoMesViewModel GetLancamentosDoMes(int mes, int ano, Guid contaId)
        {
            return Mapper.Map<LancamentosDoMes, LancamentosDoMesViewModel>(_lancamentoService.GetLancamentosDoMes(mes, ano, contaId));
        }

        #region Metodos privados
        private decimal AjustarValorParaSalvar(LancamentoViewModel lancamentoViewModel)
        {
            decimal valorAjustado;
            if (lancamentoViewModel.Transacao == TipoTransacao.Despesa && Math.Sign(lancamentoViewModel.Valor) == 1)
                valorAjustado = lancamentoViewModel.Valor * -1;
            else if (lancamentoViewModel.Transacao == TipoTransacao.Receita && Math.Sign(lancamentoViewModel.Valor) == -1)
                valorAjustado = lancamentoViewModel.Valor * -1;
            else
                valorAjustado = lancamentoViewModel.Valor;

            return valorAjustado;
        }
        private LancamentoViewModel AjustarLancamentoParaExibir(LancamentoViewModel lancamentoViewModel)
        {
            if (lancamentoViewModel.Valor > 0)
            {
                lancamentoViewModel.Transacao = TipoTransacao.Receita;
            }
            else
            {
                lancamentoViewModel.Transacao = TipoTransacao.Despesa;
                lancamentoViewModel.Valor = lancamentoViewModel.Valor * -1;
            }

            return lancamentoViewModel;
        }
        #endregion
    }
}
