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

namespace Moneta.Infra.Data.Repositories
{
    public class LancamentoRepository : RepositoryBase<Lancamento, MonetaContext>, ILancamentoRepository
    {
        public override void Remove(Lancamento lancamento)
        {
            var entry = Context.Entry(lancamento);
            DbSet.Attach(lancamento);
            entry.State = EntityState.Deleted;
        }

        public override Lancamento GetById(Guid id)
        {
            var lancamento = DbSet.Find(id);
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

        public void ImportarOfx(string caminhoOfx, Guid contaId)
        {
            PrepararOfxParaXmlLegivel(caminhoOfx);
            XElement doc = XElement.Load(caminhoOfx);
            IEnumerable<Lancamento> lancamentos = (from c in doc.Descendants("STMTTRN")
                select new Lancamento
                {
                    ContaId = contaId,
                    // TROCAR ESTE IMPROVISO
                    CategoriaId = Guid.Parse("77ad006b-ddf8-4d41-b87f-b20f48d1430a"), 
                    Valor = decimal.Parse(c.Element("TRNAMT").Value, NumberFormatInfo.InvariantInfo),
                    DataVencimento = DateTime.ParseExact(c.Element("DTPOSTED").Value.Substring(0,8), "yyyyMMdd", CultureInfo.InvariantCulture),
                    Descricao = c.Element("MEMO").Value,
                    Observacao = c.Element("MEMO").Value,
                    NumeroDocumento = c.Element("REFNUM").Value
                });

            foreach(var lancamento in lancamentos)
            {
                this.Add(lancamento);
            }
            this.Context.SaveChanges();
        }

        private void PrepararOfxParaXmlLegivel(string caminhoOfx)
        {
            string textoOfx = File.ReadAllText(caminhoOfx);
            int inicio = textoOfx.IndexOf("<OFX>");
            //int fim = textoOfx.Length;
            var textoXml = textoOfx.Substring(inicio);
            File.WriteAllText(caminhoOfx, textoXml);
        }
    }
}
