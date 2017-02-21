using Moneta.Domain.Entities;
using System;

namespace Moneta.Domain.Interfaces.Repository
{
    public interface IExtratoBancarioRepository : IRepositoryBase<ExtratoBancario>
    {
        void ImportarOfx(string caminhoOfx, Guid extratoBancarioId);
    }
}
