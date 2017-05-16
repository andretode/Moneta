using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moneta.Application.ViewModels
{
    public class GrupoPesquisaViewModel
    {
        [Required(ErrorMessage = "Preencha o campo")]
        [MaxLength(150, ErrorMessage = "Máximo {0} caracteres")]
        [MinLength(3, ErrorMessage = "Mínimo {0} caracteres")]
        [DisplayName("Descrição")]
        public string DescricaoGrupoPesquisa { get; set; }

        public IEnumerable<GrupoLancamentoViewModel> Grupos { get; set; }
    }
}
