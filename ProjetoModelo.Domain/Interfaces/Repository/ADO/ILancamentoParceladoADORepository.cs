﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moneta.Domain.Interfaces.Repository.ADO
{
    public interface ILancamentoParceladoADORepository
    {
        void ForceDelete(Guid lancamentoParceladoId);
    }
}
