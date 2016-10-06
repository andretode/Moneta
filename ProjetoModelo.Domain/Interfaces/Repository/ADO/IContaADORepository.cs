using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moneta.Domain.Entities;

namespace Moneta.Domain.Interfaces.Repository.ADO
{
    public interface IContaADORepository
    {
        IEnumerable<Conta> GetAll();
    }
}
