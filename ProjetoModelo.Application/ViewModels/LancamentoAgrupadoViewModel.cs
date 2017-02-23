using Moneta.Application.ViewModels;
using System.Collections.Generic;

namespace Moneta.Application.ViewModels
{
    public class LancamentoAgrupadoViewModel
    {
        public string Descricao { get; set; }
        public List<LancamentoViewModel> Lancamentos { get; set; }
    }
}
