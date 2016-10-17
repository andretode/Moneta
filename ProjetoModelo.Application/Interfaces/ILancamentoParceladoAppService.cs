using System;
using System.Collections.Generic;
using Moneta.Application.Validation;
using Moneta.Application.ViewModels;
using Moneta.Domain.ValueObjects;

namespace Moneta.Application.Interfaces
{
    public interface ILancamentoParceladoAppService : IDisposable
    {
        ValidationAppResult Add(LancamentoParceladoViewModel LancamentoParceladoViewModel);
        LancamentoParceladoViewModel GetById(Guid id);
        LancamentoParceladoViewModel GetByIdReadOnly(Guid id);
        IEnumerable<LancamentoParceladoViewModel> GetAll();
        IEnumerable<LancamentoParceladoViewModel> GetAllReadOnly();
        void Update(LancamentoParceladoViewModel LancamentoParceladoViewModel);
        void Remove(LancamentoParceladoViewModel LancamentoParceladoViewModel);
    }
}
