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

        public IEnumerable<Lancamento> GetLancamentosDeTodosOsGruposDoMes(int mes, int ano)
        {
            var gruposDoMes = _GrupoLancamentoRepository.GetAll().Where(g => g.DataVencimento.Month == mes && g.DataVencimento.Year == ano);
            return GetLancamentosDeTodosOsGrupos(gruposDoMes);
        }

        public IEnumerable<Lancamento> GetLancamentosDeTodosOsGruposDosMesesAnteriores(DateTime dataUltimoDiaMesAnterior)
        {
            var gruposDoMes = _GrupoLancamentoRepository.GetAll().Where(g => g.DataVencimento <= dataUltimoDiaMesAnterior);
            return GetLancamentosDeTodosOsGrupos(gruposDoMes);
        }

        private IEnumerable<Lancamento> GetLancamentosDeTodosOsGrupos(IEnumerable<GrupoLancamento> grupos)
        {
            var lancamentos = new List<Lancamento>();
            foreach (var g in grupos)
            {
                if (g.Lancamentos != null)
                    lancamentos.AddRange(g.Lancamentos);
            }

            return lancamentos;
        }
    }
}
