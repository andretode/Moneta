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
    public class ContaAppService : AppServiceBase<MonetaContext>, IContaAppService
    {
        private readonly IContaService _contaService;

        public ContaAppService(IContaService contaService)
        {
            _contaService = contaService;
        }

        public ValidationAppResult Add(ContaViewModel contaViewModel)
        {
            var conta= Mapper.Map<ContaViewModel, Conta>(contaViewModel);

            BeginTransaction();

            var result = _contaService.Adicionar(conta);
            if (!result.IsValid)
                return DomainToApplicationResult(result);
            
            Commit();

            return DomainToApplicationResult(result);
        }

        public ContaViewModel GetById(Guid id)
        {
            return Mapper.Map<Conta, ContaViewModel>(_contaService.GetById(id));
        }

        public IEnumerable<ContaViewModel> GetAll()
        {
            return Mapper.Map<IEnumerable<Conta>, IEnumerable<ContaViewModel>>(_contaService.GetAll());
        }

        public void Update(ContaViewModel contaViewModel)
        {
            var conta = Mapper.Map<ContaViewModel, Conta>(contaViewModel);

            BeginTransaction();
            _contaService.Update(conta);
            Commit();
        }

        public void Remove(ContaViewModel contaViewModel)
        {
            var conta = Mapper.Map<ContaViewModel, Conta>(contaViewModel);

            BeginTransaction();
            _contaService.Remove(conta);
            Commit();
        }

        public void Dispose()
        {
            _contaService.Dispose();
        }


        public IEnumerable<ContaViewModel> ObterContasGrid(int page, string pesquisa)
        {
            return Mapper.Map<IEnumerable<Conta>, IEnumerable<ContaViewModel>>(_contaService.ObterContasGrid(page, pesquisa));
        }


        public int ObterTotalRegistros(string pesquisa)
        {
            return _contaService.ObterTotalRegistros(pesquisa);
        }
    }
}
