﻿using System.Collections.Generic;
using Moneta.Domain.Entities;
using System;

namespace Moneta.Domain.Interfaces.Repository
{
    public interface ILancamentoRepository : IRepositoryBase<Lancamento>
    {
        Lancamento GetByIdReadOnly(Guid id);
        //IEnumerable<Lancamento> GetAll();
        IEnumerable<Lancamento> GetAll(bool ativo);
    }
}
