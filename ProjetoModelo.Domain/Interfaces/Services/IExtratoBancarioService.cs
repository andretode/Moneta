using Moneta.Domain.Entities;
using Moneta.Domain.ValueObjects;
using System;
using System.Collections.Generic;

namespace Moneta.Domain.Interfaces.Services
{
    public interface IExtratoBancarioService : IServiceBase<ExtratoBancario>
    {
        ValidationResult Adicionar(ExtratoBancario extratoBancario);
        int ImportarOfx(string caminhoOfx, Guid contaId);
        void RemoveAll(IEnumerable<ExtratoBancario> extratos);
    }
}
