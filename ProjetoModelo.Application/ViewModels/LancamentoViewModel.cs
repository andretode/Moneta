using Moneta.Infra.CrossCutting.Enums;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Moneta.Application.ViewModels
{
    public class LancamentoViewModel
    {
        public LancamentoViewModel()
        {
            LancamentoId = Guid.NewGuid();
            BaseDaSerie = false;
            DataVencimento = DateTime.Now;
            Ativo = true;
        }

        public LancamentoViewModel(DateTime MesAnoCompetencia)
        {
            LancamentoId = Guid.NewGuid();
            BaseDaSerie = false;

            if (MesAnoCompetencia.Month == DateTime.Now.Month && MesAnoCompetencia.Year == DateTime.Now.Year)
                DataVencimento = DateTime.Now;
            else
                DataVencimento = new DateTime(MesAnoCompetencia.Year, MesAnoCompetencia.Month, 1);
        }

        [Key]
        [DisplayName("Código")]
        public Guid LancamentoId { get; set; }

        [Required(ErrorMessage = "Preencha o campo")]
        [MaxLength(150, ErrorMessage = "Máximo {0} caracteres")]
        [MinLength(2, ErrorMessage = "Mínimo {0} caracteres")]
        [DisplayName("Descrição")]
        public string Descricao { get; set; }

        [DisplayName("Descrição Resumida")]
        public string DescricaoResumida {
            get {
                string descricaoResumida = "";
                if(this.DescricaoMaisNumeroParcela !=null)
                {
                    int tamanho = 35;
                    int posicaoUltimoCaracter = (this.DescricaoMaisNumeroParcela.Length >= tamanho ? tamanho : this.DescricaoMaisNumeroParcela.Length);
                    descricaoResumida = DescricaoMaisNumeroParcela.Substring(0, posicaoUltimoCaracter);

                    if (DescricaoMaisNumeroParcela.Length >= tamanho)
                        descricaoResumida += "...";
                }
                return descricaoResumida;
            }
        }

        [DisplayName("Descrição Mais N° da Parcela")]
        [JsonIgnore]
        public string DescricaoMaisNumeroParcela { get; set; }

        [DataType(DataType.Currency)]
        [Range(typeof(decimal), "-9999999,99", "9999999,99", ErrorMessage="O valor fornecido excede os limites do sistema.")]
        [Required(ErrorMessage = "Preencha o campo")]
        [DisplayName("Valor")]
        public decimal Valor { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yy}")]
        [DataType(DataType.DateTime)]
        [DisplayName("Vencimento")]
        public DateTime DataVencimento { get; set; }

        [DisplayName("N° Documento")]
        public string NumeroDocumento { get; set; }

        [DisplayName("Pago?")]
        public bool Pago { get; set; }

        [DisplayName("Observação")]
        public string Observacao { get; set; }

        [DisplayName("Transação")]
        public TipoTransacaoEnum TipoDeTransacao { get; set; }

        [DisplayName("Data de Cadastro")]
        [ScaffoldColumn(false)]
        public DateTime DataCadastro { get; set; }

        [DisplayName("Fake")]
        [ScaffoldColumn(false)]
        public bool Fake { get; set; }

        [DisplayName("Base da Série?")]
        [ScaffoldColumn(false)]
        public bool BaseDaSerie { get; set; }

        [DisplayName("Id da Parcela na Série")]
        [ScaffoldColumn(false)]
        public string IdDaParcelaNaSerie { get; set; }

        [DisplayName("Ativo?")]
        [ScaffoldColumn(false)]
        public bool Ativo { get; set; }

        [DisplayName("Categoria")]
        public Guid CategoriaId { get; set; }

        [DisplayName("Categoria")]
        [JsonIgnore]
        public virtual CategoriaViewModel Categoria { get; set; }

        [DisplayName("Conta")]
        public Guid ContaId { get; set; }

        [DisplayName("Conta")]
        [JsonIgnore]
        public virtual ContaViewModel Conta { get; set; }

        [DisplayName("Lançamento Parcelado")]
        public Guid? LancamentoParceladoId { get; set; }

        [DisplayName("Lançamento Parcelado")]
        [JsonIgnore]
        public virtual LancamentoParceladoViewModel LancamentoParcelado { get; set; }

        [DisplayName("Transferência")]
        public Guid? LancamentoIdTransferencia { get; set; }

        [DisplayName("Transferência")]
        [JsonIgnore]
        public virtual LancamentoViewModel LancamentoTransferencia { get; set; }

        [DisplayName("Grupo")]
        public Guid? GrupoLancamentoId { get; set; }

        [DisplayName("Grupo")]
        [JsonIgnore]
        public virtual GrupoLancamentoViewModel GrupoLancamento { get; set; }

        [DisplayName("Extrato Bancário")]
        public Guid? ExtratoBancarioId { get; set; }

        [DisplayName("Extrato Bancário")]
        [JsonIgnore]
        public virtual ExtratoBancarioViewModel ExtratoBancario { get; set; } 
    }
}