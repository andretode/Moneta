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
                ArrayDeSaldos = JsonConvert.SerializeObject(listaSaldoPorCategoria.Select(d => Math.Abs(d.Saldo)).ToArray());
            }
        }

        public string ArrayDeCategorias { get; set; }
        public string ArrayDeCores { get; set; }
        public string ArrayDeSaldos { get; set; }
    }
}
