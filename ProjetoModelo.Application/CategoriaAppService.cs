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
    public class CategoriaAppService : AppServiceBase<MonetaContext>, ICategoriaAppService
    {
        private readonly ICategoriaService _categoriaService;

        public CategoriaAppService(ICategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        public ValidationAppResult Add(CategoriaViewModel categoriaViewModel)
        {
            var categoria= Mapper.Map<CategoriaViewModel, Categoria>(categoriaViewModel);

            BeginTransaction();

            var result = _categoriaService.Adicionar(categoria);
            if (!result.IsValid)
                return DomainToApplicationResult(result);
            
            Commit();

            return DomainToApplicationResult(result);
        }

        public CategoriaViewModel GetById(Guid id)
        {
            return Mapper.Map<Categoria, CategoriaViewModel>(_categoriaService.GetById(id));
        }

        public IEnumerable<CategoriaViewModel> GetAll()
        {
            return Mapper.Map<IEnumerable<Categoria>, IEnumerable<CategoriaViewModel>>(_categoriaService.GetAll());
        }

        public IEnumerable<CategoriaViewModel> GetAllReadOnly()
        {
            return Mapper.Map<IEnumerable<Categoria>, IEnumerable<CategoriaViewModel>>(_categoriaService.GetAllReadOnly());
        }

        public void Update(CategoriaViewModel categoriaViewModel)
        {
            var categoria = Mapper.Map<CategoriaViewModel, Categoria>(categoriaViewModel);

            BeginTransaction();
            _categoriaService.Update(categoria);
            Commit();
        }

        public void Remove(CategoriaViewModel categoriaViewModel)
        {
            var categoria = Mapper.Map<CategoriaViewModel, Categoria>(categoriaViewModel);

            BeginTransaction();
            _categoriaService.Remove(categoria);
            Commit();
        }

        public void Dispose()
        {
            _categoriaService.Dispose();
        }


        //public IEnumerable<CategoriaViewModel> ObterCategoriasGrid(int page, string pesquisa)
        //{
        //    return Mapper.Map<IEnumerable<Categoria>, IEnumerable<CategoriaViewModel>>(_categoriaService.ObterCategoriasGrid(page, pesquisa));
        //}


        //public int ObterTotalRegistros(string pesquisa)
        //{
        //    return _categoriaService.ObterTotalRegistros(pesquisa);
        //}
    }
}
