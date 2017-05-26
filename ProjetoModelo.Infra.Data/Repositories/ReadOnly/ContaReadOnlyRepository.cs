using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Moneta.Domain.Entities;
using Moneta.Domain.Interfaces.Repository.ReadOnly;

namespace Moneta.Infra.Data.Repositories.ReadOnly
{
    public class ContaReadOnlyRepository : RepositoryBaseReadOnly, IContaReadOnlyRepository
    {


        public Conta GetById(Guid id)
        {
            using (IDbConnection cn = Connection)
            {
                cn.Open();

                var sql = @"Select * From Conta c " +
                    @"WHERE c.ContaId ='" + id + "'";

                var conta = cn.Query<Conta>(sql);

                return conta.FirstOrDefault();
            }
        }
      
        public IEnumerable<Conta> GetAll()
        {
            using (IDbConnection cn = Connection)
            {
                cn.Open();

                var sql = @"SELECT * FROM Conta
                            ORDER BY Descricao asc";

                var contas = cn.Query<Conta>(sql);

                return contas;
            }
        }

        public IEnumerable<Conta> ObterContasGrid(int page, string pesquisa, int tamanhoPagina=10)
        {
            using (IDbConnection cn = Connection)
            {
                var sql = "";
                if (pesquisa == null || pesquisa == "")
                {
                    sql = @"SELECT * FROM Conta
                            ORDER BY Descricao asc";
                }
                else
                {
                    sql = @"SELECT * FROM Conta
                            WHERE (Descricao  Like '%" + pesquisa + @"%')
                            ORDER BY Descricao asc";
                }

                if(page > 0)
                {
                    sql += " LIMIT " + (page * tamanhoPagina) + "," + tamanhoPagina;
                }

                cn.Open();
                return cn.Query<Conta>(sql);
            }
        }


        public int ObterTotalRegistros(string pesquisa)
        {
            using (IDbConnection cn = Connection)
            {
                cn.Open();

                var sql = "";
                var total = 0;
                if (pesquisa == null || pesquisa == "")
                {
                    sql = @"SELECT COUNT(*) FROM Conta";
                    total = Int32.Parse(cn.ExecuteScalar(sql).ToString());
                }
                else
                {
                    sql = @"SELECT COUNT(*) FROM Conta WHERE Descricao Like '%" + pesquisa + "%'";
                    total = Int32.Parse(cn.ExecuteScalar(sql, new { nomePesquisa = pesquisa }).ToString());
                }

                return total;
            }
        }
    }
}
