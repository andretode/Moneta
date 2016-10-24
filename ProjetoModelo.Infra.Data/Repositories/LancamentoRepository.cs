using System.Data.Entity;
using Moneta.Domain.Entities;
using Moneta.Domain.Interfaces.Repository;
using Moneta.Infra.Data.Context;
using System.Linq;
using System;
using System.Collections.Generic;

namespace Moneta.Infra.Data.Repositories
{
    public class LancamentoRepository : RepositoryBase<Lancamento, MonetaContext>, ILancamentoRepository
    {
        public void UpdateEmSerie(Lancamento lancamentoEditado)
        {
            string strDataVencimentoAnterior = lancamentoEditado.IdDaParcelaNaSerie.Substring(36, 10);
            var dataVencimentoAnterior = DateTime.Parse(strDataVencimentoAnterior);
            var diasDiff = (lancamentoEditado.DataVencimento - dataVencimentoAnterior).TotalDays;

            var lancamentos = GetAllReadOnly().Where(l => l.LancamentoParceladoId == lancamentoEditado.LancamentoParceladoId);            
            foreach(var l in lancamentos)
            {
                l.Descricao = lancamentoEditado.Descricao;
                l.Valor = lancamentoEditado.Valor;
                l.CategoriaId = lancamentoEditado.CategoriaId;
                l.DataVencimento = l.DataVencimento.AddDays(diasDiff);
                if(!l.BaseDaSerie)
                    l.IdDaParcelaNaSerie = l.IdDaParcelaNaSerie.Substring(0, 36) + l.DataVencimento;
                Update(l);
            }
        }

        public override void Remove(Lancamento lancamento)
        {
            var entry = Context.Entry(lancamento);
            DbSet.Attach(lancamento);
            entry.State = EntityState.Deleted;
        }

        public Lancamento GetByIdReadOnly(Guid id)
        {
            base.Context.SetProxyCreationEnabledToFalse();

            try {
                return DbSet.AsNoTracking().Where(x => x.LancamentoId == id).First();
            }
            catch {}

            return null;
        }

        public override IEnumerable<Lancamento> GetAll()
        {
            return GetAll(false);
        }

        public IEnumerable<Lancamento> GetAll(bool somenteOsAtivo)
        {
            if (somenteOsAtivo)
                return DbSet.Where(l => l.Ativo == true);
            else
                return DbSet;
        }
    }
}
