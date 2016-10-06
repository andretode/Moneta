using System;
using System.Collections.Generic;
using Moneta.Application.Validation;
using Moneta.Application.ViewModels;

namespace Moneta.Application.Interfaces
{
    public interface IContaAppService : IDisposable
    {
        ValidationAppResult Add(ContaViewModel contaViewModel);
        ContaViewModel GetById(Guid id);
        IEnumerable<ContaViewModel> GetAll();
        void Update(ContaViewModel contaViewModel);
        void Remove(ContaViewModel contaViewModel);
        int ObterTotalRegistros(string pesquisa);
        IEnumerable<ContaViewModel> ObterContasGrid(int page, string pesquisa);
    }
}
