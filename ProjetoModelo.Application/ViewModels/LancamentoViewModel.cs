﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Moneta.Application.ViewModels
{
    public class LancamentoViewModel
    {
        public LancamentoViewModel()
        {
            LancamentoId = Guid.NewGuid();
        }

        [Key]
        [DisplayName("Código")]
        public Guid LancamentoId { get; set; }

        [Required(ErrorMessage = "Preencha o campo")]
        [MaxLength(150, ErrorMessage = "Máximo {0} caracteres")]
        [MinLength(2, ErrorMessage = "Mínimo {0} caracteres")]
        [DisplayName("Descrição")]
        public string Descricao { get; set; }

        [DataType(DataType.Currency)]
        [Range(typeof(decimal), "0", "99999999999")]
        [Required(ErrorMessage = "Preencha o campo")]
        [DisplayName("Valor")]
        public decimal Valor { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [DisplayName("Data de Vencimento")]
        public DateTime DataVencimento { get; set; }

        [DisplayName("Pago?")]
        public bool Pago { get; set; }

        [DisplayName("Data de Cadastro")]
        [ScaffoldColumn(false)]
        public DateTime DataCadastro { get; set; }

        public Guid CategoriaId { get; set; }
        public virtual CategoriaViewModel Categoria { get; set; }
        public Guid ContaId { get; set; }
        [DisplayName("Conta")]
        public virtual ContaViewModel Conta { get; set; }
    }
}