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
    public class LancamentoParceladoAppService : AppServiceBase<MonetaContext>, ILancamentoParceladoAppService
    {
        private readonly ILancamentoParceladoService _lancamentoService;

        public LancamentoParceladoAppService(ILancamentoParceladoService LancamentoParceladoService)
        {
            _lancamentoService = LancamentoParceladoService;
        }

        public ValidationAppResult Add(LancamentoParceladoViewModel lancamentoViewModel)
        {
            var LancamentoParcelado = Mapper.Map<LancamentoParceladoViewModel, LancamentoParcelado>(lancamentoViewModel);

            BeginTransaction();

            var result = _lancamentoService.Adicionar(LancamentoParcelado);
            if (!result.IsValid)
                return DomainToApplicationResult(result);
            
            Commit();

            return DomainToApplicationResult(result);
        }

        public LancamentoParceladoViewModel GetById(Guid id)
        {
            var lancamentoVM = Mapper.Map<LancamentoParcelado, LancamentoParceladoViewModel>(_lancamentoService.GetById(id));
            return lancamentoVM;
        }

        public LancamentoParceladoViewModel GetByIdReadOnly(Guid id)
        {
            var lancamentoVM = Mapper.Map<LancamentoParcelado, LancamentoParceladoViewModel>(_lancamentoService.GetByIdReadOnly(id));
            return lancamentoVM;
        }

        public IEnumerable<LancamentoParceladoViewModel> GetAll()
        {
            return Mapper.Map<IEnumerable<LancamentoParcelado>, IEnumerable<LancamentoParceladoViewModel>>(_lancamentoService.GetAll());
        }

        public IEnumerable<LancamentoParceladoViewModel> GetAllReadOnly()
        {
            return Mapper.Map<IEnumerable<LancamentoParcelado>, IEnumerable<LancamentoParceladoViewModel>>(_lancamentoService.GetAllReadOnly());
        }

        public void Update(LancamentoParceladoViewModel lancamentoViewModel)
        {
            var LancamentoParcelado = Mapper.Map<LancamentoParceladoViewModel, LancamentoParcelado>(lancamentoViewModel);

            BeginTransaction();
            _lancamentoService.Update(LancamentoParcelado);
            Commit();
        }

        public void Remove(LancamentoParceladoViewModel LancamentoParceladoViewModel)
        {
            var LancamentoParcelado = Mapper.Map<LancamentoParceladoViewModel, LancamentoParcelado>(LancamentoParceladoViewModel);

            BeginTransaction();
            _lancamentoService.Remove(LancamentoParcelado);
            Commit();
        }

        public void Dispose()
        {
            _lancamentoService.Dispose();
        }
    }
}
