using Moneta.Domain.Entities;
using Moneta.Domain.Interfaces.Specification;
using Moneta.Domain.ValueObjects;
using System;
using System.Text.RegularExpressions;

namespace Moneta.Domain.Specification.Lancamentos
{
    class LancamentoPossuiIdAppUser : ISpecification<AgregadoLancamentosDoMes>
    {
        public bool IsSatisfiedBy(AgregadoLancamentosDoMes agregadoLancamentosDoMes)
        {
            return agregadoLancamentosDoMes.AppUserIdFiltro != null && agregadoLancamentosDoMes.AppUserIdFiltro != Guid.Empty;
        }
    }
}
