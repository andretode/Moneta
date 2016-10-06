using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moneta.Domain.Entities;
using Moneta.Domain.Interfaces.Specification;

namespace Moneta.Domain.Specification.Contas
{
    class ContaPossuiStatusAtivo : ISpecification<Conta>
    {
        public bool IsSatisfiedBy(Conta conta)
        {
            return conta.Ativo;
        }
    }
}
