using Moneta.Domain.ValueObjects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moneta.Application.ViewModels
{
    public class GraficoSaldoPorCategoriaViewModel
    {
        public GraficoSaldoPorCategoriaViewModel(List<SaldoPorCategoria> listaSaldoPorCategoria)
        {
            if (listaSaldoPorCategoria != null)
            {
                ArrayDeCategorias = JsonConvert.SerializeObject(listaSaldoPorCategoria.Select(d => d.Categoria).ToArray());
                ArrayDeCores = JsonConvert.SerializeObject(listaSaldoPorCategoria.Select(d => d.CorHex).ToArray());
                ArrayDeSaldosRealizados = JsonConvert.SerializeObject(listaSaldoPorCategoria.Select(d => Math.Abs(d.Saldo)).ToArray());
                ArrayDeOrcamentos = JsonConvert.SerializeObject(listaSaldoPorCategoria.Select(c => c.OrcamentoMensal).ToArray());
            }
        }

        public string ArrayDeCategorias { get; set; }
        public string ArrayDeCores { get; set; }
        public string ArrayDeSaldosRealizados { get; set; }
        public string ArrayDeOrcamentos { get; set; }
    }
}
