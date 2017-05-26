using Moneta.Domain.Entities;
using Moneta.Domain.Interfaces.Repository;
using Moneta.Domain.Interfaces.Repository.ADO;
using Moneta.Domain.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Moneta.Domain.Services
{
    public class LancamentoConciliacaoService : LancamentoService, ILancamentoConciliacaoService
    {
        public LancamentoConciliacaoService(
            IGrupoLancamentoRepository GrupoLancamentoRepository,
            ILancamentoParceladoRepository LancamentoParceladoRepository,
            ILancamentoParceladoADORepository LancamentoParceladoADORepository,
            ILancamentoRepository LancamentoRepository)
            : base(GrupoLancamentoRepository, LancamentoParceladoRepository, LancamentoParceladoADORepository, LancamentoRepository)
        {
        }

        public IEnumerable<LancamentoAgrupado> GetLancamentosSugeridosParaConciliacao(ExtratoBancario extrato)
        {
            const decimal porc = 0.2M;
            decimal maxValor = extrato.Valor * (1 - porc);
            decimal minValor = extrato.Valor * (1 + porc);

            const int dias = 7;
            DateTime maxData = extrato.DataCompensacao.AddDays(dias);
            DateTime minData = extrato.DataCompensacao.AddDays(-dias);

            if (extrato.Valor > 0)
            {
                minValor = extrato.Valor * (1 - porc);
                maxValor = extrato.Valor * (1 + porc);
            }

            var lancamentosUnicos = GetLancamentosUnicos(extrato, maxValor, minValor, maxData, minData);
            var lancamentosDeGrupos = GetLancamentosDeGrupos(extrato, maxValor, minValor, maxData, minData);
            var lancamentosUnicosMaisOsDeGrupo = lancamentosUnicos.Union(lancamentosDeGrupos);

            return this._LancamentoDoMesService.AgruparLancamentos(lancamentosUnicosMaisOsDeGrupo);
        }

        private IEnumerable<Lancamento> GetLancamentosUnicos(ExtratoBancario extrato, decimal maxValor, decimal minValor, DateTime maxData, DateTime minData)
        {
            var lancamentoMaisFake = new LancamentoMaisFakeService(_LancamentoParceladoRepository, _LancamentoRepository);
            var todosNaoConciliados = lancamentoMaisFake.GetAllMaisFake(extrato.DataCompensacao.Month, extrato.DataCompensacao.Year)
                .Where(l => l.ExtratoBancarioId == null && l.GrupoLancamentoId == null);
            var sugeridosComValoresProximos = todosNaoConciliados.Where(l => l.Valor > minValor && l.Valor < maxValor)
                .OrderBy(l => l.DataVencimento);
            var sugeridosComDatasProximas = sugeridosComValoresProximos.Where(l => l.DataVencimento >= minData
                && l.DataVencimento <= maxData);

            return sugeridosComDatasProximas;
        }

        private IEnumerable<Lancamento> GetLancamentosDeGrupos(ExtratoBancario extrato, decimal maxValor, decimal minValor, DateTime maxData, DateTime minData)
        {
            var gruposDeLancamento = _GrupoLancamentoRepository.GetAll()
                .Where(l => l.ExtratoBancarioId == null && l.Valor > minValor && l.Valor < maxValor)
                .OrderBy(l => l.DataVencimento);

            var lancamentosDeGrupos = new List<Lancamento>();
            foreach (var grupo in gruposDeLancamento)
            {
                lancamentosDeGrupos.AddRange(grupo.Lancamentos);
            }

            return lancamentosDeGrupos;
        }
    }
}
