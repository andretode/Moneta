using System.Data.Entity;
using Moneta.Domain.Entities;
using Moneta.Domain.Interfaces.Repository;
using Moneta.Infra.Data.Context;
using Moneta.Infra.CrossCutting.Enums;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Globalization;
using System.IO;
using System.Text;
using Moneta.Infra.Data.Repositories.ADO;

namespace Moneta.Infra.Data.Repositories
{
    public class LancamentoRepository : RepositoryBase<Lancamento, MonetaContext>, ILancamentoRepository
    {
        private readonly LancamentoADORepository _LancamentoADORepository;

        public LancamentoRepository()
        {
            _LancamentoADORepository = new LancamentoADORepository();
        }

        public override void Remove(Lancamento lancamento)
        {
            var id = lancamento.LancamentoParceladoId;
            lancamento.LancamentoParcelado = null;
            var entry = Context.Entry(lancamento);
            DbSet.Attach(lancamento);
            entry.State = EntityState.Deleted;
        }

        public void ForceRemove(Guid id)
        {
            _LancamentoADORepository.ForceRemove(id);
        }

        public override Lancamento GetById(Guid id)
        {
            var lancamento = DbSet.Find(id);
            if (lancamento != null)
                lancamento.LancamentoTransferencia = DbSet.Find(lancamento.LancamentoIdTransferencia);
            return lancamento;
        }

        public Lancamento GetByIdReadOnly(Guid id)
        {
            base.Context.SetProxyCreationEnabledToFalse();

            try
            {
                return DbSet.AsNoTracking().Where(x => x.LancamentoId == id).First();
            }
            catch { }

            return null;
        }

        public override IEnumerable<Lancamento> GetAll()
        {
            return GetAll(true, false) ;
        }

        public IEnumerable<Lancamento> GetAll(bool somenteOsAtivo, bool asNoTracking)
        {
            if (somenteOsAtivo)
            {
                if (asNoTracking)
                {
                    base.Context.SetProxyCreationEnabledToFalse();
                    return DbSet.AsNoTracking().Where(l => l.Ativo == true);
                }
                else
                    return DbSet.Where(l => l.Ativo == true);
            }
            else
            {
                if (asNoTracking)
                {
                    base.Context.SetProxyCreationEnabledToFalse();
                    return DbSet.AsNoTracking();
                }
                else
                    return DbSet;
            }
        }
    }
}
