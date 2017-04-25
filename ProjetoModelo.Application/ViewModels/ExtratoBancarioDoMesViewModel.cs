using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moneta.Application.ViewModels
{
    public class ExtratoBancarioDoMesViewModel
    {
        public Guid? contaIdFiltro = Guid.Empty;

        [DisplayName("Filtro Conta")]
        public Guid? ContaIdFiltro
        {
            get { return contaIdFiltro; }
            set { contaIdFiltro = value; }
        }


        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yy}")]
        [DataType(DataType.DateTime)]
        [DisplayName("Mês/Ano")]
        public DateTime MesAnoCompetencia { get; set; }

        public virtual IEnumerable<ExtratoBancarioViewModel> ExtratosDoMes { get; set; }
    }
}
