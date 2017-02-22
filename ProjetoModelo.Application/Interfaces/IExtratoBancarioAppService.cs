using System;
using System.Collections.Generic;
using Moneta.Application.Validation;
using Moneta.Application.ViewModels;

namespace Moneta.Application.Interfaces
{
    public interface IExtratoBancarioAppService : IDisposable
    {
        //ValidationAppResult Add(ExtratoBancarioViewModel extratoBancarioViewModel);
        //ContaViewModel GetById(Guid id);
        IEnumerable<ExtratoBancarioViewModel> GetAll();
        //void Update(ContaViewModel contaViewModel);
        //void Remove(ContaViewModel contaViewModel);
        void ImportarOfx(string caminhoOfx, Guid contaId);
    }
}
