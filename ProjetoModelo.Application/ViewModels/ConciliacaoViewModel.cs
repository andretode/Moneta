using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moneta.Application.ViewModels
{
    public class ConciliacaoViewModel
    {
        public IEnumerable<LancamentoAgrupadoViewModel> Lancamentos { get; set; }
        public ExtratoBancarioViewModel ExtratoBancario { get; set; }
    }
}
