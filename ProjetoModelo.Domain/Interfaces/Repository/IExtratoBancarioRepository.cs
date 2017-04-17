using Moneta.Domain.Entities;
using System;

namespace Moneta.Domain.Interfaces.Repository
{
    public interface IExtratoBancarioRepository : IRepositoryBase<ExtratoBancario>
    {
        int ImportarOfx(string caminhoOfx, Guid contaId);
    }
}
