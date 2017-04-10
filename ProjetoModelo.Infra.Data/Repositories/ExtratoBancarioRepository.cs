using System.Data.Entity;
using Moneta.Domain.Entities;
using Moneta.Domain.Interfaces.Repository;
using Moneta.Infra.Data.Context;
using System;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.IO;

namespace Moneta.Infra.Data.Repositories
{
    public class ExtratoBancarioRepository : RepositoryBase<ExtratoBancario, MonetaContext>, IExtratoBancarioRepository
    {
        public override void Remove(ExtratoBancario extratoBancario)
        {
            var entry = Context.Entry(extratoBancario);
            DbSet.Attach(extratoBancario);
            entry.State = EntityState.Deleted;
        }

        public void ImportarOfx(string caminhoOfx, Guid contaId)
        {
            PrepararOfxParaXmlLegivel(caminhoOfx);
            XElement doc = XElement.Load(caminhoOfx);
            IEnumerable<ExtratoBancario> extratosBancarios = (from c in doc.Descendants("STMTTRN")
                select new ExtratoBancario
                {
                    ContaId = contaId,
                    Valor = decimal.Parse(c.Element("TRNAMT").Value, NumberFormatInfo.InvariantInfo),
                    DataCompensacao = DateTime.ParseExact(c.Element("DTPOSTED").Value.Substring(0, 8), "yyyyMMdd", CultureInfo.InvariantCulture),
                    Descricao = c.Element("MEMO").Value,
                    NumeroDocumento = (c.Element("REFNUM") != null ? c.Element("REFNUM").Value : c.Element("FITID").Value)
                });

            foreach (var lancamento in extratosBancarios)
            {
                this.Add(lancamento);
            }
            this.Context.SaveChanges();
        }

        private void PrepararOfxParaXmlLegivel(string caminhoOfx)
        {
            string textoOfx = File.ReadAllText(caminhoOfx, Encoding.GetEncoding(1252));
            int inicio = textoOfx.IndexOf("<OFX>");
            var textoXml = textoOfx.Substring(inicio);
            File.WriteAllText(caminhoOfx, textoXml);
        }
    }
}
