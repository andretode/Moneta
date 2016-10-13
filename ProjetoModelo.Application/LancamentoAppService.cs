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

namespace Moneta.Application
{
    public class LancamentoAppService : AppServiceBase<MonetaContext>, ILancamentoAppService
    {
        private readonly ILancamentoService _LancamentoService;

        public LancamentoAppService(ILancamentoService LancamentoService)
        {
            _LancamentoService = LancamentoService;
        }

        public ValidationAppResult Add(LancamentoViewModel LancamentoViewModel)
        {
            var Lancamento= Mapper.Map<LancamentoViewModel, Lancamento>(LancamentoViewModel);

            BeginTransaction();

            var result = _LancamentoService.Adicionar(Lancamento);
            if (!result.IsValid)
                return DomainToApplicationResult(result);
            
            Commit();

            return DomainToApplicationResult(result);
        }

        public LancamentoViewModel GetById(Guid id)
        {
            return Mapper.Map<Lancamento, LancamentoViewModel>(_LancamentoService.GetById(id));
        }

        public LancamentoViewModel GetByIdReadOnly(Guid id)
        {
            return Mapper.Map<Lancamento, LancamentoViewModel>(_LancamentoService.GetByIdReadOnly(id));
        }

        public IEnumerable<LancamentoViewModel> GetAll()
        {
            return Mapper.Map<IEnumerable<Lancamento>, IEnumerable<LancamentoViewModel>>(_LancamentoService.GetAll());
        }

        public IEnumerable<LancamentoViewModel> GetAllReadOnly()
        {
            return Mapper.Map<IEnumerable<Lancamento>, IEnumerable<LancamentoViewModel>>(_LancamentoService.GetAllReadOnly());
        }

        public void Update(LancamentoViewModel LancamentoViewModel)
        {
            var Lancamento = Mapper.Map<LancamentoViewModel, Lancamento>(LancamentoViewModel);

            BeginTransaction();
            _LancamentoService.Update(Lancamento);
            Commit();
        }

        public void Remove(LancamentoViewModel LancamentoViewModel)
        {
            var Lancamento = Mapper.Map<LancamentoViewModel, Lancamento>(LancamentoViewModel);

            BeginTransaction();
            _LancamentoService.Remove(Lancamento);
            Commit();
        }

        public void Dispose()
        {
            _LancamentoService.Dispose();
        }
    }
}
