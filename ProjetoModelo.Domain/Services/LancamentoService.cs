using System;
using System.Collections.Generic;
using System.Linq;
using Moneta.Domain.Entities;
using Moneta.Domain.Interfaces.Repository;
using Moneta.Domain.Interfaces.Repository.ADO;
using Moneta.Domain.Interfaces.Repository.ReadOnly;
using Moneta.Domain.Interfaces.Services;
using Moneta.Domain.ValueObjects;
using Moneta.Infra.CrossCutting.Enums;

namespace Moneta.Domain.Services
{
    public class LancamentoService : ServiceBase<Lancamento>, ILancamentoService
    {
        private readonly ILancamentoRepository _LancamentoRepository;
        private readonly ILancamentoParceladoRepository _LancamentoParceladoRepository;
        private readonly IGrupoLancamentoRepository _GrupoLancamentoRepository;

        public LancamentoService(
            IGrupoLancamentoRepository GrupoLancamentoRepository,
            ILancamentoParceladoRepository LancamentoParceladoRepository,
            ILancamentoRepository LancamentoRepository)
            : base(LancamentoRepository)
        {
            _GrupoLancamentoRepository = GrupoLancamentoRepository;
            _LancamentoParceladoRepository = LancamentoParceladoRepository;
            _LancamentoRepository = LancamentoRepository;
        }

        public override Lancamento GetById(Guid id)
        {
            return _LancamentoRepository.GetById(id);
        }

        public Lancamento GetByIdReadOnly(Guid id)
        {
            return _LancamentoRepository.GetByIdReadOnly(id);
        }

        public ValidationResult Adicionar(Lancamento lancamento)
        {
            var resultadoValidacao = new ValidationResult();

            if (!lancamento.IsValid())
            {
                resultadoValidacao.AdicionarErro(lancamento.ResultadoValidacao);
                return resultadoValidacao;
            }

            base.Add(lancamento);

            return resultadoValidacao;
        }

        public override void Update(Lancamento lancamento)
        {
            if (lancamento.TipoDeTransacao == TipoTransacaoEnum.Transferencia && lancamento.LancamentoTransferencia != null)
            {
                var lancamentoTransferenciaPar = lancamento.LancamentoTransferencia;
                lancamentoTransferenciaPar.Valor = lancamento.Valor * -1;
                _LancamentoRepository.Update(lancamentoTransferenciaPar);
            }

            _LancamentoRepository.Update(lancamento);
        }

        public void UpdateEmSerie(Lancamento lancamentoEditado)
        {
            var lancamentoUpdateEmSerieService = new LancamentoUpdateEmSerieService(_LancamentoParceladoRepository, _LancamentoRepository);
            lancamentoUpdateEmSerieService.UpdateEmSerie(lancamentoEditado);
        }

        public void RemoveEmSerie(Lancamento lancamentoEditado)
        {
            var lancamentoDeleteEmSerieService = new LancamentoDeleteEmSerieService(_LancamentoParceladoRepository, _LancamentoRepository);
            lancamentoDeleteEmSerieService.DeleteEmSerie(lancamentoEditado);
        }

        public void RemoveTransferencia(Lancamento lancamento)
        {
            var lancamentoPar = _LancamentoRepository.GetByIdReadOnly((Guid)lancamento.LancamentoIdTransferencia);
            _LancamentoRepository.Remove(lancamentoPar);
            _LancamentoRepository.Remove(lancamento);
        }

        public AgregadoLancamentosDoMes GetLancamentosDoMes(AgregadoLancamentosDoMes lancamentosDoMes)
        {
            var mes = lancamentosDoMes.MesAnoCompetencia.Month;
            var ano = lancamentosDoMes.MesAnoCompetencia.Year;
            var contaId = lancamentosDoMes.ContaIdFiltro;
            var dataUltimoDiaMesAnterior = GetDataUltimoDiaMesAnterior(mes, ano);
            var agregadoLancamentosDoMes = new AgregadoLancamentosDoMes();
            
            var lancamentosMesAnteriorTodasAsContas = _LancamentoRepository.GetAll().Where(l => l.DataVencimento <= dataUltimoDiaMesAnterior);

            var aux = lancamentosMesAnteriorTodasAsContas.Where(l => l.ContaId == contaId).OrderByDescending(l => l.DataVencimento);

            if (lancamentosDoMes.ContaIdFiltro == Guid.Empty)
                agregadoLancamentosDoMes.SaldoDoMesAnterior = lancamentosMesAnteriorTodasAsContas.Where(l => l.BaseDaSerie == false).Sum(l => l.Valor); // Remove da soma o lançamento que é base da série
            else
                agregadoLancamentosDoMes.SaldoDoMesAnterior = lancamentosMesAnteriorTodasAsContas.Where(l => l.ContaId == contaId && l.BaseDaSerie == false).Sum(l => l.Valor); // Remove da soma o lançamento que é base da série

            var lancamentoMaisFake = new LancamentoMaisFakeService(_LancamentoParceladoRepository, _LancamentoRepository);
            var lancamentosDoMesTodasAsContasMaisFake = lancamentoMaisFake.GetAllMaisFake(mes, ano);
            if (lancamentosDoMes.ContaIdFiltro == Guid.Empty)
                agregadoLancamentosDoMes.LancamentosDoMes = lancamentosDoMesTodasAsContasMaisFake.OrderBy(l => l.DataVencimento).ThenBy(l => l.DataCadastro);
            else
                agregadoLancamentosDoMes.LancamentosDoMes = lancamentosDoMesTodasAsContasMaisFake.Where(l => l.ContaId == contaId).OrderBy(l => l.DataVencimento).ThenBy(l => l.DataCadastro);

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

            return AgruparLancamentos(agregadoLancamentosDoMes);
        }

        private AgregadoLancamentosDoMes AgruparLancamentos(AgregadoLancamentosDoMes agregadoLancamentosDoMes)
        {
            var lancamentosAgrupados = new List<LancamentoAgrupado>();

            //agrupa os lançamentos associados a um grupo de lançamentos
            var lancamentosGrupo = agregadoLancamentosDoMes.LancamentosDoMes
                .Where(l => l.GrupoLancamentoId != null).GroupBy(l => l.GrupoLancamentoId);
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
            var lancamentosSemGrupo = agregadoLancamentosDoMes.LancamentosDoMes
                .Where(l => l.GrupoLancamentoId == null);
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

            agregadoLancamentosDoMes.LancamentosAgrupados = agrupamentoOrdenado;
            return agregadoLancamentosDoMes;
        }

        public IEnumerable<Lancamento> GetLancamentosSugeridosParaConciliacao(ExtratoBancario extrato)
        {
            const decimal porc = 0.2M;
            decimal maxValor = extrato.Valor * (1 - porc);
            decimal minValor = extrato.Valor * (1+porc);

            const int dias = 7;
            DateTime maxData = extrato.DataCompensacao.AddDays(dias);
            DateTime minData = extrato.DataCompensacao.AddDays(-dias);

            if(extrato.Valor > 0)
            {
                minValor = extrato.Valor * (1 - porc);
                maxValor = extrato.Valor * (1 + porc);
            }

            var lancamentoMaisFake = new LancamentoMaisFakeService(_LancamentoParceladoRepository, _LancamentoRepository);
            var todosNaoConciliados = lancamentoMaisFake.GetAllMaisFake(extrato.DataCompensacao.Month, extrato.DataCompensacao.Year)
                .Where(l => l.ExtratoBancarioId == null);
            var sugeridosComValoresProximos = todosNaoConciliados.Where(l => l.Valor > minValor && l.Valor < maxValor)
                .OrderBy(l => l.DataVencimento);
            var sugeridosComDatasProximos = sugeridosComValoresProximos.Where(l => l.DataVencimento >= minData
                && l.DataVencimento <= maxData);
            return sugeridosComDatasProximos;
        }

        /// <summary>
        /// Busca as receitas, despesas e saldos da movimentação financeira de uma conta por dia
        /// </summary>
        /// <param name="lancamentosDoMes">Dados informados pelo usuário para filtrar a pesquisa</param>
        /// <param name="resumido">Informe true caso queria um conjunto de dados somente no dia que houve movimentações financeira</param>
        /// <returns>Retorna um conjunto de dados de receitas, despesas e saldos de uma conta por dia. A Tuple é na sequencia: data da movimentação financeira, receita, despesa e saldo.</returns>
        public List<Tuple<DateTime, decimal, decimal, decimal>> GetSaldoDoMesPorDia(AgregadoLancamentosDoMes lancamentosDoMes, bool resumido)
        {
            var listaDeSaldoPorDia = new List<Tuple<DateTime, decimal, decimal, decimal>>();
            var agregadoLancamentosDoMes = GetLancamentosDoMes(lancamentosDoMes);
            var lancamentos = agregadoLancamentosDoMes.LancamentosDoMes;

            var mes = lancamentosDoMes.MesAnoCompetencia.Month;
            var ano = lancamentosDoMes.MesAnoCompetencia.Year;

            List<DateTime> arrayDataVencimento;
            if (resumido)
                arrayDataVencimento = SomenteDiasDoMesComMovimentacao(mes, ano, lancamentos);
            else
                arrayDataVencimento = TodosDiasDoMes(mes, ano);

            decimal receitaAcumulada = 0;
            decimal despesaAcumulada = 0;
            decimal saldoAcumulado = 0;
            decimal saldoDoMesAnterior = agregadoLancamentosDoMes.SaldoDoMesAnterior;
            if (saldoDoMesAnterior > 0)
                receitaAcumulada = saldoDoMesAnterior;
            else
                despesaAcumulada = saldoDoMesAnterior;
            foreach (var dia in arrayDataVencimento)
            {
                receitaAcumulada += lancamentos.Where(l => l.DataVencimento == dia && l.TipoDeTransacao == TipoTransacaoEnum.Receita).Sum(l => l.Valor);
                despesaAcumulada += lancamentos.Where(l => l.DataVencimento == dia && l.TipoDeTransacao == TipoTransacaoEnum.Despesa).Sum(l => l.Valor);
                saldoAcumulado = receitaAcumulada + despesaAcumulada;
                listaDeSaldoPorDia.Add(new Tuple<DateTime, decimal, decimal, decimal>(dia, receitaAcumulada, Math.Abs(despesaAcumulada), saldoAcumulado));
            }

            return listaDeSaldoPorDia;
        }

        #region Metodos Privados
        private List<DateTime> SomenteDiasDoMesComMovimentacao(int mes, int ano, IEnumerable<Lancamento> lancamentos)
        {
            var arrayDataVencimento = new List<DateTime>();
            arrayDataVencimento.Add(new DateTime(ano, mes, 1));
            arrayDataVencimento.AddRange(lancamentos.Select(l => l.DataVencimento).Distinct().ToList());
            arrayDataVencimento.Add(new DateTime(ano, mes, DateTime.DaysInMonth(ano, mes)));
            arrayDataVencimento = arrayDataVencimento.Distinct().ToList();

            return arrayDataVencimento;
        }

        private List<DateTime> TodosDiasDoMes(int mes, int ano)
        {
            var datas = new List<DateTime>();
            var dataFinal = new DateTime(ano, mes, DateTime.DaysInMonth(ano, mes));
            var dataCounter = new DateTime(ano, mes, 1);
            while (dataCounter <= dataFinal)
            {
                datas.Add(dataCounter);
                dataCounter = dataCounter.AddDays(1);
            }
            return datas;
        }

        private DateTime GetDataUltimoDiaMesAnterior(int mesAtual, int anoAtual)
        {
            var mesAnoAtual = new DateTime(anoAtual, mesAtual, 1);
            var mesAnterior = mesAnoAtual.AddMonths(-1);
            return new DateTime(mesAnterior.Year, mesAnterior.Month, DateTime.DaysInMonth(mesAnterior.Year, mesAnterior.Month));
        }
        #endregion
    }
}
