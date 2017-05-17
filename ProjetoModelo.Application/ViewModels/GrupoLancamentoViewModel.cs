using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Moneta.Application.ViewModels
{
    public class GrupoLancamentoViewModel
    {
        public GrupoLancamentoViewModel()
        {
            GrupoLancamentoId = Guid.NewGuid();
        }

        [Key]
        [DisplayName("Código")]
        public Guid GrupoLancamentoId { get; set; }

        [Required(ErrorMessage = "Preencha o campo")]
        [MaxLength(150, ErrorMessage = "Máximo {0} caracteres")]
        [MinLength(2, ErrorMessage = "Mínimo {0} caracteres")]
        [DisplayName("Descrição")]
        public string Descricao { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yy}")]
        [DataType(DataType.DateTime)]
        [DisplayName("Vencimento")]
        public DateTime DataVencimento { get; set; }

        [DisplayName("Pago?")]
        public bool Pago { get; set; }

        [DisplayName("Valor")]
        [UIHint("DinheiroColorido")]
        public decimal Valor { get; private set; }

        [DisplayName("Conta")]
        public Guid ContaId { get; set; }

        [DisplayName("Conta")]
        [JsonIgnore]
        public virtual ContaViewModel Conta { get; set; }

        [DisplayName("Extrato Bancário")]
        public Guid? ExtratoBancarioId { get; set; }

        [DisplayName("Extrato Bancário")]
        [JsonIgnore]
        public virtual ExtratoBancarioViewModel ExtratoBancario { get; set; }

        public Guid? GrupoLancamentoIdPai { get; set; }

        [DisplayName("Data de Cadastro")]
        [ScaffoldColumn(false)]
        public DateTime DataCadastro { get; set; }
        
        public ICollection<LancamentoViewModel> Lancamentos { get; set; }
        public ICollection<GrupoLancamentoViewModel> GruposDeLancamentos { get; set; }

        [MaxLength(100, ErrorMessage = "Máximo {0} caracteres")]
        [MinLength(2, ErrorMessage = "Mínimo {0} caracteres")]
        [DisplayName("N° Doc")]
        public string NumeroDocumento { get; set; }

        public Guid? LancamentoIdDividido { get; set; }
    }
}