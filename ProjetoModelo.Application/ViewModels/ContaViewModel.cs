using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Moneta.Application.ViewModels
{
    public class ContaViewModel
    {
        public ContaViewModel()
        {
            ContaId = Guid.NewGuid();
        }

        [Key]
        [DisplayName("Código")]
        public Guid ContaId { get; set; }

        [Required(ErrorMessage = "Preencha o campo")]
        [MaxLength(150, ErrorMessage = "Máximo {0} caracteres")]
        [MinLength(2, ErrorMessage = "Mínimo {0} caracteres")]
        [DisplayName("Descrição")]
        public string Descricao { get; set; }

        [DisplayName("Data de Cadastro")]
        [ScaffoldColumn(false)]
        public DateTime DataCadastro { get; set; }

        [DisplayName("Ativo?")]
        public bool Ativo { get; set; }

        public ICollection<LancamentoViewModel> Lancamentos { get; set; }
    }
}