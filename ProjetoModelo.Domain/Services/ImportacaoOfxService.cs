using Moneta.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Moneta.Domain.Services
{
    public class ImportacaoOfxService
    {
        public static IEnumerable<IExtratoOfx> ImportarNovosExtratosOfx(string caminhoOfx, Guid contaId, IEnumerable<IExtratoOfx> extratosExistentesNoMes, DateTime mesAnoCompetencia, bool cartao = false)
        {
            PrepararOfxParaXmlLegivel(caminhoOfx);
            XElement doc = XElement.Load(caminhoOfx);
            List<ExtratoBancario> extratosBancarios = 
                (from c in doc.Descendants("STMTTRN")
                 select new ExtratoBancario
                {
                    ContaId = contaId,
                    Valor = decimal.Parse(c.Element("TRNAMT").Value, NumberFormatInfo.InvariantInfo),
                    DataCompensacao = DateTime.ParseExact(c.Element("DTPOSTED").Value.Substring(0, 8), "yyyyMMdd", CultureInfo.InvariantCulture),
                    Descricao = c.Element("MEMO").Value,
                    NumeroDocumento = (c.Element("REFNUM") != null ? c.Element("REFNUM").Value : c.Element("FITID").Value)
                }).ToList();

            IEnumerable<ExtratoBancario> extratosBancariosDoMes;
            if(cartao)
            {
                extratosBancarios.RemoveAt(0); //Remove o primeiro item que refere-se ao pagamento do cartão do mês anterior

                //Pega datas também do mês anterior, visto que o cartão registra compras feitas naquele mês.
                extratosBancariosDoMes = extratosBancarios.Where(e =>
                (e.DataCompensacao.Month == mesAnoCompetencia.Month || e.DataCompensacao.Month == mesAnoCompetencia.Month - 1)
                && e.DataCompensacao.Year == mesAnoCompetencia.Year);
            }
            else
                extratosBancariosDoMes = extratosBancarios.Where(e => e.DataCompensacao.Month == mesAnoCompetencia.Month
                    && e.DataCompensacao.Year == mesAnoCompetencia.Year);

            var extratosNovosNaoImportados = extratosBancariosDoMes.Where(e => !extratosExistentesNoMes.Any(e2 => e2.NumeroDocumento == e.NumeroDocumento));

            return extratosNovosNaoImportados;
        }

        private static void PrepararOfxParaXmlLegivel(string caminhoOfx)
        {
            string textoOfx = File.ReadAllText(caminhoOfx, Encoding.GetEncoding(1252));
            int inicio = textoOfx.IndexOf("<OFX>");
            var textoXml = textoOfx.Substring(inicio);
            File.WriteAllText(caminhoOfx, textoXml);
        }
    }
}
