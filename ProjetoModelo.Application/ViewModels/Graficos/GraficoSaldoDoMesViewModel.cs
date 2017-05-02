using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moneta.Application.ViewModels
{
    public class GraficoSaldoDoMesViewModel
    {

        public GraficoSaldoDoMesViewModel(List<Tuple<DateTime, decimal, decimal, decimal>> dadosDoGrafico)
        {
            if (dadosDoGrafico != null)
            {
                ArrayDeDias = dadosDoGrafico.First().Item1.ToString("dd");
                ArrayDeReceitas = dadosDoGrafico.First().Item2.ToString().Replace(',', '.');
                ArrayDeDespesas = dadosDoGrafico.First().Item3.ToString().Replace(',', '.');
                ArrayDeSaldos = dadosDoGrafico.First().Item4.ToString().Replace(',', '.');

                for (int i = 1; i < dadosDoGrafico.Count; i++)
                {
                    ArrayDeDias += "," + dadosDoGrafico[i].Item1.ToString("dd");
                    ArrayDeReceitas += "," + dadosDoGrafico[i].Item2.ToString().Replace(',', '.');
                    ArrayDeDespesas += "," + dadosDoGrafico[i].Item3.ToString().Replace(',', '.');
                    ArrayDeSaldos += "," + dadosDoGrafico[i].Item4.ToString().Replace(',', '.');
                }
            }
        }

        public string ArrayDeDias { get; set; }
        public string ArrayDeReceitas { get; set; }
        public string ArrayDeDespesas { get; set; }
        public string ArrayDeSaldos { get; set; }
    }

}
