using System;
using System.Collections.Generic;

namespace Moneta.Application.ViewModels
{
    public class GraficosViewModel
    {
        public List<Tuple<DateTime, decimal, decimal, decimal>> GraficoSaldoDoMes { get; set; }
        public List<Tuple<string, decimal>> GraficoSaldoPorCategoria { get; set; }
    }
}
