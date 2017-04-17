using System.Data.Entity;
using Moneta.Domain.Entities;
using Moneta.Domain.Interfaces.Repository;
using Moneta.Infra.Data.Context;
using System;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Globalization;
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

        public int ImportarOfx(string caminhoOfx, Guid contaId)
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

            var mes = extratosBancarios.First().DataCompensacao.Month;
            var ano = extratosBancarios.First().DataCompensacao.Year;
            var naoImportados = GetExtratosNovosNaoImportados(mes, ano, extratosBancarios);
            var quant = naoImportados.Count();
            foreach (var extrato in naoImportados)
            {
                this.Add(extrato);
            }
            this.Context.SaveChanges();

            return quant;
        }

        private IEnumerable<ExtratoBancario> GetExtratosNovosNaoImportados(int mes, int ano, IEnumerable<ExtratoBancario> extratosNovos)
        {
            var extratosJaCadastrados = DbSet.AsNoTracking().Where(e => e.DataCompensacao.Month == mes && e.DataCompensacao.Year == ano);
            return extratosNovos.Where(e => !extratosJaCadastrados.Any(e2 => e2.NumeroDocumento == e.NumeroDocumento));
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
