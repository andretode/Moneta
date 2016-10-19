using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moneta.Domain.Entities
{
    public class LancamentoComparer: IEqualityComparer<Lancamento>
    {
        public bool Equals(Lancamento x, Lancamento y)
        {
            return
                x.DataVencimento == y.DataVencimento &&
                x.Descricao == y.Descricao &&
                x.Valor == y.Valor &&
                x.ContaId == y.ContaId &&
                x.CategoriaId == y.CategoriaId &&
                x.LancamentoParceladoId == y.LancamentoParceladoId;
        }

        public int GetHashCode(Lancamento obj)
        {
            throw new NotImplementedException();
        }
    }
}
