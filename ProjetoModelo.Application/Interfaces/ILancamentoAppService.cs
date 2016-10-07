﻿using System;
using System.Collections.Generic;
using Moneta.Application.Validation;
using Moneta.Application.ViewModels;

namespace Moneta.Application.Interfaces
{
    public interface ILancamentoAppService : IDisposable
    {
        ValidationAppResult Add(LancamentoViewModel LancamentoViewModel);
        LancamentoViewModel GetById(Guid id);
        IEnumerable<LancamentoViewModel> GetAll();
        IEnumerable<LancamentoViewModel> GetAllReadOnly();
        void Update(LancamentoViewModel LancamentoViewModel);
        void Remove(LancamentoViewModel LancamentoViewModel);
    }
}
