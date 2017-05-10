using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moneta.Domain.ValueObjects
{
    public class SaldoPorCategoria
    {
        public string Categoria { get; set; }
        public string CorHex { get; set; }
        public decimal Saldo { get; set; }
        public decimal OrcamentoMensal { get; set; }
    }
}
