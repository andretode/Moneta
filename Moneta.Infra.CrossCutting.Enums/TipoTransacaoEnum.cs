using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moneta.Infra.CrossCutting.Enums
{
    public enum TipoTransacaoEnum {
        //[DescriptionAttribute("Receita")] 
        Receita = 1,
        //[DescriptionAttribute("Despesa")] 
        Despesa = 2,
        //[DescriptionAttribute("Transferência")] 
        Transferencia = 3
    }
}
