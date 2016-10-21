using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moneta.Application.ViewModels
{
    public class GraficoSaldoViewModel
    {
        public List<SaldoNoDiaViewModel> SaldoPorDia { get; set; }
    }

    public class SaldoNoDiaViewModel
    {
        public decimal Saldo { get; set; }
        public DateTime Dia { get; set; }
    }
}
