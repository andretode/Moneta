using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moneta.Domain.Entities
{
    public interface IExtratoOfx
    {
        string Descricao { get; set; }
        decimal Valor { get; set; }
        DateTime DataCompensacao { get; set; }
        string NumeroDocumento { get; set; }
        Guid ContaId { get; set; }
        bool WasImported(IExtratoOfx extrato);
    }
}
