using Moneta.Domain.Entities;
using Moneta.Domain.ValueObjects;
using System;

namespace Moneta.Domain.Interfaces.Services
{
    public interface IExtratoBancarioService : IServiceBase<ExtratoBancario>
    {
        ValidationResult Adicionar(ExtratoBancario extratoBancario);
        void ImportarOfx(string caminhoOfx, Guid contaId);
    }
}
