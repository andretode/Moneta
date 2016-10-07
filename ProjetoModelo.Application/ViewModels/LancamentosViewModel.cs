using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Moneta.Application.ViewModels
{
    public class LancamentosViewModel
    {
        [DisplayName("Filtro Conta")]
        [Required(ErrorMessage = "Selecione uma conta")]
        public Guid ContaIdFiltro { get; set; }

        public virtual IEnumerable<LancamentoViewModel> Lancamentos { get; set; }
    }
}