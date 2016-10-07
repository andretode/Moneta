using System;
using System.Collections.Generic;
using Moneta.Application.Validation;
using Moneta.Application.ViewModels;

namespace Moneta.Application.Interfaces
{
    public interface ICategoriaAppService : IDisposable
    {
        ValidationAppResult Add(CategoriaViewModel categoriaViewModel);
        CategoriaViewModel GetById(Guid id);
        IEnumerable<CategoriaViewModel> GetAll();
        IEnumerable<CategoriaViewModel> GetAllReadOnly();
        void Update(CategoriaViewModel categoriaViewModel);
        void Remove(CategoriaViewModel categoriaViewModel);
        //int ObterTotalRegistros(string pesquisa);
        //IEnumerable<CategoriaViewModel> ObterCategoriasGrid(int page, string pesquisa);
    }
}
