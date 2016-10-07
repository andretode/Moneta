using Moneta.Domain.Entities;
using Moneta.Domain.Specification.Categorias;
using Moneta.Domain.Validation.Base;

namespace Moneta.Domain.Validation.Categorias
{
    public class CategoriaEstaAptaParaCadastroNoSistema : FiscalBase<Categoria>
    {
        public CategoriaEstaAptaParaCadastroNoSistema()
        {
            var possuiCor = new CategoriaPossuiCorValida();

            base.AdicionarRegra("PossuiCor", new Regra<Categoria>(possuiCor, "Categoria não possui cor válida"));
        }
    }
}
