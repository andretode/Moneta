using Moneta.Application.ViewModels;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Moneta.Application.ViewModels
{
    public class LancamentoAgrupadoViewModel
    {
        public string Descricao { get; set; }
        public List<LancamentoViewModel> Lancamentos { get; set; }
    }
}
