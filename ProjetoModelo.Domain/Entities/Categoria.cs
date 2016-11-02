using Moneta.Domain.Validation.Categorias;
using Moneta.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moneta.Domain.Entities
{
    public class Categoria
    {
        public Categoria()
        {
            CategoriaId = Guid.NewGuid();
        }

        public Guid CategoriaId { get; set; }
        public string Descricao { get; set; }
        public string Cor { get; set; }
        public virtual ICollection<Lancamento> Lancamentos { get; set; }
        public DateTime DataCadastro { get; set; }
        public const string Nenhum = "Nenhum";
        public ValidationResult ResultadoValidacao { get; private set; }

        public bool IsValid()
        {
            var fiscal = new CategoriaEstaAptaParaCadastroNoSistema();

            ResultadoValidacao = fiscal.Validar(this);

            return ResultadoValidacao.IsValid;
        }
    }
}
