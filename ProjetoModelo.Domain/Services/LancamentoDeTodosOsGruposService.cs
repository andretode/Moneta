using Moneta.Domain.Entities;
using Moneta.Domain.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Moneta.Domain.Services
{
    public class LancamentoDeTodosOsGruposService
    {
        private readonly IGrupoLancamentoRepository _GrupoLancamentoRepository;

        public LancamentoDeTodosOsGruposService(IGrupoLancamentoRepository GrupoLancamentoRepository)
        {
            _GrupoLancamentoRepository = GrupoLancamentoRepository;
        }

        public IEnumerable<Lancamento> GetLancamentosDeTodosOsGruposDoMes(Guid appUserId, int mes, int ano)
        {
            var gruposDoMes = _GrupoLancamentoRepository.GetAll(appUserId).Where(g => g.DataVencimento.Month == mes && g.DataVencimento.Year == ano && g.GrupoLancamentoIdPai == null);
            return GetLancamentosDeTodosOsGrupos(gruposDoMes);
        }

        public IEnumerable<Lancamento> GetLancamentosDeTodosOsGruposDosMesesAnteriores(Guid appUserId, DateTime dataUltimoDiaMesAnterior)
        {
            var gruposDoMes = _GrupoLancamentoRepository.GetAll(appUserId).Where(g => g.DataVencimento <= dataUltimoDiaMesAnterior && g.GrupoLancamentoIdPai == null);
            return GetLancamentosDeTodosOsGrupos(gruposDoMes);
        }

        private IEnumerable<Lancamento> GetLancamentosDeTodosOsGrupos(IEnumerable<GrupoLancamento> grupos)
        {
            var lancamentos = new List<Lancamento>();
            
            foreach (var g in grupos)
            {
                // Pega os lançamentos únicos
                if (g.Lancamentos != null)
                    lancamentos.AddRange(g.Lancamentos);

                // Pega os lançamentos dos grupos filhos
                if(g.GruposDeLancamentos != null)
                {
                    foreach(var g2 in g.GruposDeLancamentos)
                    {
                        lancamentos.AddRange(g2.Lancamentos);
                    }
                }
            }

            return lancamentos;
        }
    }
}
