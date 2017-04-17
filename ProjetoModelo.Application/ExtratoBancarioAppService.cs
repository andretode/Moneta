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
    public class ExtratoBancarioAppService : AppServiceBase<MonetaContext>, IExtratoBancarioAppService
    {
        private readonly IExtratoBancarioService _ExtratoBancarioService;

        public ExtratoBancarioAppService(IExtratoBancarioService ExtratoBancarioService)
        {
            _ExtratoBancarioService = ExtratoBancarioService;
        }
        
        public ValidationAppResult Add(ExtratoBancarioViewModel ExtratoBancarioViewModel)
        {
            var ExtratoBancario= Mapper.Map<ExtratoBancarioViewModel, ExtratoBancario>(ExtratoBancarioViewModel);

            BeginTransaction();

            var result = _ExtratoBancarioService.Adicionar(ExtratoBancario);
            if (!result.IsValid)
                return DomainToApplicationResult(result);
            
            Commit();

            return DomainToApplicationResult(result);
        }

        public ExtratoBancarioViewModel GetById(Guid id)
        {
            return Mapper.Map<ExtratoBancario, ExtratoBancarioViewModel>(_ExtratoBancarioService.GetById(id));
        }

        public IEnumerable<ExtratoBancarioViewModel> GetAll()
        {
            return Mapper.Map<IEnumerable<ExtratoBancario>, IEnumerable<ExtratoBancarioViewModel>>(_ExtratoBancarioService.GetAll());
        }

        public IEnumerable<ExtratoBancarioViewModel> GetAllReadOnly()
        {
            return Mapper.Map<IEnumerable<ExtratoBancario>, IEnumerable<ExtratoBancarioViewModel>>(_ExtratoBancarioService.GetAllReadOnly());
        }

        public void Update(ExtratoBancarioViewModel ExtratoBancarioViewModel)
        {
            var ExtratoBancario = Mapper.Map<ExtratoBancarioViewModel, ExtratoBancario>(ExtratoBancarioViewModel);

            BeginTransaction();
            _ExtratoBancarioService.Update(ExtratoBancario);
            Commit();
        }

        public int ImportarOfx(string caminhoOfx, Guid contaId)
        {
            BeginTransaction();
            var quantidade = _ExtratoBancarioService.ImportarOfx(caminhoOfx, contaId);
            Commit();

            return quantidade;
        }

        public void Remove(ExtratoBancarioViewModel ExtratoBancarioViewModel)
        {
            var ExtratoBancario = Mapper.Map<ExtratoBancarioViewModel, ExtratoBancario>(ExtratoBancarioViewModel);

            BeginTransaction();
            _ExtratoBancarioService.Remove(ExtratoBancario);
            Commit();
        }

        public void RemoveAll(IEnumerable<ExtratoBancarioViewModel> extratos)
        {
            var extratosBancarios = Mapper.Map<IEnumerable<ExtratoBancarioViewModel>, IEnumerable<ExtratoBancario>>(extratos);

            BeginTransaction();
            _ExtratoBancarioService.RemoveAll(extratosBancarios);
            Commit();
        }

        public void Dispose()
        {
            _ExtratoBancarioService.Dispose();
        }
    }
}
