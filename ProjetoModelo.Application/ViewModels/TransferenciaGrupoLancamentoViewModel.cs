using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Moneta.Application.ViewModels
{
    public class TransferenciaGrupoLancamentoViewModel
    {
        public TransferenciaGrupoLancamentoViewModel()
        {
        }

        public TransferenciaGrupoLancamentoViewModel(Guid grupoLancamentoId)
        {
            GrupoLancamentoId = grupoLancamentoId;
        }

        [Required(ErrorMessage = "Preencha o campo")]
        [MaxLength(36, ErrorMessage = "Máximo {0} caracteres")]
        [MinLength(36, ErrorMessage = "Mínimo {0} caracteres")]
        [DisplayName("Id da Transferência")]
        public string TransferenciaId { get; set; }
        public Guid GrupoLancamentoId { get; set; }
    }
}
