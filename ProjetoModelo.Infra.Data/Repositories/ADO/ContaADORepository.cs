using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moneta.Domain.Entities;
using Moneta.Domain.Interfaces.Repository.ADO;

namespace Moneta.Infra.Data.Repositories.ADO
{
    public class ContaADORepository : BaseADORepository, IContaADORepository
    {
        public IEnumerable<Conta> GetAll()
        {
            var query = "SELECT * FROM CONTA";
            var contaList = new List<Conta>();

            using (IDataReader reader = Connection.ExecuteReader(CommandType.Text, query))
            {
                while (reader.Read())
                {
                    var conta = new Conta();
                    conta.ContaId = Guid.Parse(reader["ContaId"].ToString());
                    conta.Descricao = reader["Descricao"].ToString();

                    contaList.Add(conta);
                }
            }

            return contaList;
        }
    }
}
