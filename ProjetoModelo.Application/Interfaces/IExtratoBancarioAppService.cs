using System;
using System.Collections.Generic;
using Moneta.Application.Validation;
using Moneta.Application.ViewModels;

namespace Moneta.Application.Interfaces
{
    public interface IExtratoBancarioAppService : IDisposable
    {
        //ValidationAppResult Add(ExtratoBancarioViewModel extratoBancarioViewModel);
        ExtratoBancarioViewModel GetById(Guid id);
        IEnumerable<ExtratoBancarioViewModel> GetExtratosDoMes(DateTime mesAnoCompetencia, Guid contaId);
        void Remove(ExtratoBancarioViewModel extratoBancarioViewModel);
        int ImportarOfx(string caminhoOfx, Guid contaId, DateTime mesAnoCompetencia);
        void RemoveAll(IEnumerable<ExtratoBancarioViewModel> extratos);
    }
}
