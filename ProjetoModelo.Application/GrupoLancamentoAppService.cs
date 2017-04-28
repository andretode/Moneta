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
    public class GrupoLancamentoAppService : AppServiceBase<MonetaContext>, IGrupoLancamentoAppService
    {
        private readonly IGrupoLancamentoService _GrupoLancamentoService;

        public GrupoLancamentoAppService(IGrupoLancamentoService GrupoLancamentoService)
        {
            _GrupoLancamentoService = GrupoLancamentoService;
        }

        public ValidationAppResult Add(GrupoLancamentoViewModel GrupoLancamentoViewModel)
        {
            var GrupoLancamento= Mapper.Map<GrupoLancamentoViewModel, GrupoLancamento>(GrupoLancamentoViewModel);

            BeginTransaction();

            var result = _GrupoLancamentoService.Adicionar(GrupoLancamento);
            if (!result.IsValid)
                return DomainToApplicationResult(result);
            
            Commit();

            return DomainToApplicationResult(result);
        }

        public GrupoLancamentoViewModel GetById(Guid id)
        {
            return Mapper.Map<GrupoLancamento, GrupoLancamentoViewModel>(_GrupoLancamentoService.GetById(id));
        }

        public GrupoLancamentoViewModel GetByIdReadOnly(Guid id)
        {
            return Mapper.Map<GrupoLancamento, GrupoLancamentoViewModel>(_GrupoLancamentoService.GetByIdReadOnly(id));
        }

        public IEnumerable<GrupoLancamentoViewModel> GetAll()
        {
            return Mapper.Map<IEnumerable<GrupoLancamento>, IEnumerable<GrupoLancamentoViewModel>>(_GrupoLancamentoService.GetAll());
        }

        public IEnumerable<GrupoLancamentoViewModel> GetAllReadOnly()
        {
            return Mapper.Map<IEnumerable<GrupoLancamento>, IEnumerable<GrupoLancamentoViewModel>>(_GrupoLancamentoService.GetAllReadOnly());
        }

        public void Update(GrupoLancamentoViewModel GrupoLancamentoViewModel)
        {
            var GrupoLancamento = Mapper.Map<GrupoLancamentoViewModel, GrupoLancamento>(GrupoLancamentoViewModel);

            BeginTransaction();
            _GrupoLancamentoService.Update(GrupoLancamento);
            Commit();
        }
        
        public void Remove(GrupoLancamentoViewModel GrupoLancamentoViewModel)
        {
            var GrupoLancamento = Mapper.Map<GrupoLancamentoViewModel, GrupoLancamento>(GrupoLancamentoViewModel);

            BeginTransaction();
            _GrupoLancamentoService.Remove(GrupoLancamento);
            Commit();
        }

        public void Dispose()
        {
            _GrupoLancamentoService.Dispose();
        }
    }
}
