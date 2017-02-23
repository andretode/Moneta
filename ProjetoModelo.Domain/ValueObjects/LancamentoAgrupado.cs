using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moneta.Domain.Entities
{
    public class LancamentoAgrupado
    {
        public string Descricao { get; set; }
        public List<Lancamento> Lancamentos { get; set; }
    }
}
