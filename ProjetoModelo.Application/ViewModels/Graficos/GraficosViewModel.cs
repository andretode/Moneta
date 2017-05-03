using System;
using System.Collections.Generic;

namespace Moneta.Application.ViewModels
{
    public class GraficosViewModel
    {
        public GraficoSaldoDoMesViewModel GraficoSaldoDoMes { get; set; }
        public GraficoSaldoPorCategoriaViewModel GraficoSaldoPorCategoria { get; set; }

        public Guid? ContaIdFiltro { get; set; }
    }
}
