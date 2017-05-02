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
        public GraficoSaldoPorCategoriaViewModel(List<Tuple<string, decimal>> dadosDoGrafico)
        {
            if (dadosDoGrafico != null)
            {
                ArrayDeCategorias =  JsonConvert.SerializeObject(dadosDoGrafico.Select(d => d.Item1).ToArray());
                ArrayDeSaldos = JsonConvert.SerializeObject(dadosDoGrafico.Select(d => Math.Abs(d.Item2)).ToArray());
            }
        }

        public string ArrayDeCategorias { get; set; }
        public string ArrayDeSaldos { get; set; }
    }
}
