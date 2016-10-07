using Moneta.Domain.Entities;
using Moneta.Domain.Interfaces.Specification;
using System.Text.RegularExpressions;

namespace Moneta.Domain.Specification.Categorias
{
    class CategoriaPossuiCorValida : ISpecification<Categoria>
    {
        public bool IsSatisfiedBy(Categoria conta)
        {
            Regex r = new Regex("^#(?:[0-9a-fA-F]{3}){1,2}$", RegexOptions.IgnoreCase);
            Match m = r.Match(conta.Cor);
            return m.Success;
        }
    }
}
