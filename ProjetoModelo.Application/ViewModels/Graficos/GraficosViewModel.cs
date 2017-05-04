using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Moneta.Application.ViewModels
{
    public class GraficosViewModel
    {
        public GraficosViewModel()
        {
            ContaIdFiltro = Guid.Empty;
            MesAnoCompetencia = DateTime.Now;
        }

        public GraficoSaldoDoMesViewModel GraficoSaldoDoMes { get; set; }
        public GraficoSaldoPorCategoriaViewModel GraficoSaldoPorCategoria { get; set; }

        public Guid ContaIdFiltro { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayName("Mês/Ano")]
        public DateTime MesAnoCompetencia { get; set; }

        public bool SomentePagos { get; set; }
    }
}
