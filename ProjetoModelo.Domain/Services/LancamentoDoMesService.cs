using Moneta.Domain.Entities;
using Moneta.Domain.Interfaces.Repository;
using Moneta.Domain.ValueObjects;
using Moneta.Infra.CrossCutting.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Moneta.Domain.Services
{
    public class LancamentoDoMesService
    {
        protected readonly ILancamentoRepository _LancamentoRepository;
        protected readonly ILancamentoParceladoRepository _LancamentoParceladoRepository;
        protected readonly IGrupoLancamentoRepository _GrupoLancamentoRepository;
        protected readonly LancamentoMaisFakeService _LancamentoMaisFakeService;
        protected readonly LancamentoDeTodosOsGruposService _LancamentoDeTodosOsGruposService;

        public LancamentoDoMesService(
            ILancamentoRepository LancamentoRepository,
            ILancamentoParceladoRepository LancamentoParceladoRepository,
            IGrupoLancamentoRepository GrupoLancamentoRepository)
        {
            _LancamentoRepository = LancamentoRepository;
            _LancamentoParceladoRepository = LancamentoParceladoRepository;
            _GrupoLancamentoRepository = GrupoLancamentoRepository;
            _LancamentoMaisFakeService = new LancamentoMaisFakeService(_LancamentoParceladoRepository, _LancamentoRepository);
            _LancamentoDeTodosOsGruposService = new LancamentoDeTodosOsGruposService(_GrupoLancamentoRepository);
        }

        public AgregadoLancamentosDoMes GetLancamentosDoMes(Guid appUserId, AgregadoLancamentosDoMes lancamentosDoMes)
        {
            var mes = lancamentosDoMes.MesAnoCompetencia.Month;
            var ano = lancamentosDoMes.MesAnoCompetencia.Year;
            var contaId = lancamentosDoMes.ContaIdFiltro;
            var dataUltimoDiaMesAnterior = GetDataUltimoDiaMesAnterior(mes, ano);
            var agregadoLancamentosDoMes = new AgregadoLancamentosDoMes();

            agregadoLancamentosDoMes.SaldoDoMesAnterior = CalcularSaldoDoMesAnterior(dataUltimoDiaMesAnterior, contaId);
            agregadoLancamentosDoMes.LancamentosDoMes = GetTodosLancamentosDoMes(appUserId, mes, ano, contaId);

            agregadoLancamentosDoMes.SaldoDoMes = agregadoLancamentosDoMes.LancamentosDoMes.Sum(l => l.Valor)
                + agregadoLancamentosDoMes.SaldoDoMesAnterior;
            agregadoLancamentosDoMes.SaldoAtualDoMes = agregadoLancamentosDoMes.LancamentosDoMes.Where(l => l.Pago == true).Sum(l => l.Valor)
                + agregadoLancamentosDoMes.SaldoDoMesAnterior;

            agregadoLancamentosDoMes.ReceitaTotal = agregadoLancamentosDoMes.LancamentosDoMes.Where(l => l.TipoDeTransacao == TipoTransacaoEnum.Receita).Sum(l => l.Valor);
            agregadoLancamentosDoMes.DespesaTotal = agregadoLancamentosDoMes.LancamentosDoMes.Where(l => l.TipoDeTransacao == TipoTransacaoEnum.Despesa).Sum(l => l.Valor);

            if (lancamentosDoMes.PesquisarDescricao != null)
                agregadoLancamentosDoMes.LancamentosDoMes = agregadoLancamentosDoMes.LancamentosDoMes.Where(l =>
                    l.Descricao.ToLower().Contains(lancamentosDoMes.PesquisarDescricao.ToLower()) ||
                    l.DataVencimento.ToString("dd/MM/yy").Contains(lancamentosDoMes.PesquisarDescricao.ToLower())
                    );

            agregadoLancamentosDoMes.LancamentosAgrupados = AgruparLancamentos(agregadoLancamentosDoMes.LancamentosDoMes);

            return agregadoLancamentosDoMes;
        }

        private DateTime GetDataUltimoDiaMesAnterior(int mesAtual, int anoAtual)
        {
            var mesAnoAtual = new DateTime(anoAtual, mesAtual, 1);
            var mesAnterior = mesAnoAtual.AddMonths(-1);
            return new DateTime(mesAnterior.Year, mesAnterior.Month, 
                DateTime.DaysInMonth(mesAnterior.Year, mesAnterior.Month),
                23, 59, 59, 999);
        }

        private decimal CalcularSaldoDoMesAnterior(DateTime dataUltimoDiaMesAnterior, Guid contaId)
        {
            var lancamentosMesAnteriorExcetoDeGrupos = _LancamentoRepository.GetAll()
                .Where(l => l.DataVencimento <= dataUltimoDiaMesAnterior 
                    && !l.BaseDaSerie 
                    && l.GrupoLancamentoId == null);

            var lancamentosDeTodosOsGrupos = _LancamentoDeTodosOsGruposService.GetLancamentosDeTodosOsGruposDosMesesAnteriores(dataUltimoDiaMesAnterior);
            var lancamentosDoMesAnterior = lancamentosMesAnteriorExcetoDeGrupos.Union(lancamentosDeTodosOsGrupos);

            if (contaId != Guid.Empty)
                lancamentosDoMesAnterior = lancamentosDoMesAnterior.Where(l => l.ContaId == contaId);

            return lancamentosDoMesAnterior.Sum(l => l.Valor);
        }

        private IEnumerable<Lancamento> GetTodosLancamentosDoMes(Guid appUserId, int mes, int ano, Guid contaId)
        {
            var lancamentosDoMesExcetoDeGrupos = _LancamentoMaisFakeService
                .GetAllMaisFake(appUserId, mes, ano)
                .Where(l => l.GrupoLancamentoId == null);

            var lancamentosDeTodosOsGrupos = _LancamentoDeTodosOsGruposService.GetLancamentosDeTodosOsGruposDoMes(mes, ano);
            var lancamentosDoMes = lancamentosDoMesExcetoDeGrupos.Union(lancamentosDeTodosOsGrupos);

            if (contaId != Guid.Empty)
                lancamentosDoMes = lancamentosDoMes.Where(l => l.ContaId == contaId).ToList();

            return lancamentosDoMes.OrderBy(l => l.DataVencimento).ThenBy(l => l.DataCadastro);
        }

        public IEnumerable<LancamentoAgrupado> AgruparLancamentos(IEnumerable<Lancamento> lancamentos)
        {
            var lancamentosAgrupados = new List<LancamentoAgrupado>();

            //agrupa os lançamentos associados a um grupo de lançamentos
            var lancamentosGrupo = lancamentos.Where(l => l.GrupoLancamentoId != null).GroupBy(l => l.GrupoLancamentoId);
            foreach (var lancamentoGrupo in lancamentosGrupo)
            {
                var la = new LancamentoAgrupado();
                la.Descricao = lancamentoGrupo.First().GrupoLancamento.Descricao;

                la.Lancamentos = new List<Lancamento>();
                foreach (var l in lancamentoGrupo)
                    la.Lancamentos.Add(l);

                lancamentosAgrupados.Add(la);
            }

            //inclui os demais lançamentos que não estão associados a um grupo de lançamentos
            var lancamentosSemGrupo = lancamentos.Where(l => l.GrupoLancamentoId == null);
            foreach (var lancamento in lancamentosSemGrupo)
            {
                var la = new LancamentoAgrupado();
                la.Descricao = lancamento.Descricao;
                la.Lancamentos = new List<Lancamento>();
                la.Lancamentos.Add(lancamento);
                lancamentosAgrupados.Add(la);
            }

            //ordena pela data de vencimento do lançamento, porém, quando agrupado, pega a data de vencimento do agrupamento
            var agrupamentoOrdenado = from lg in lancamentosAgrupados
                                      orderby
                                        lg.Lancamentos.FirstOrDefault().GrupoLancamentoId == null ?
                                        lg.Lancamentos.FirstOrDefault().DataVencimento :
                                        lg.Lancamentos.FirstOrDefault().GrupoLancamento.DataVencimento
                                      select lg;

            return agrupamentoOrdenado;
        }
    }
}
