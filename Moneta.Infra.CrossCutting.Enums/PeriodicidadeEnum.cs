using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moneta.Infra.CrossCutting.Enums
{
    public enum PeriodicidadeEnum
    {
        //[DescriptionAttribute("Diário")]
        Diario = 1,
        //[DescriptionAttribute("Semanal")]
        Semanal = 7,
        //[DescriptionAttribute("Quinzenal")] 
        Quinzenal = 14,
        //[DescriptionAttribute("Mensal")] 
        Mensal = 30,
        //[DescriptionAttribute("Trimestral")] 
        Trimestral = 60,
        //[DescriptionAttribute("Semestral")] 
        Semestral = 180,
        //[DescriptionAttribute("Anual")] 
        Anual = 365
    }
}
