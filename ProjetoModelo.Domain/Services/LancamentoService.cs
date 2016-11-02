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

        public LancamentoService(
            ILancamentoParceladoRepository LancamentoParceladoRepository,
            ILancamentoRepository LancamentoRepository)
            : base(LancamentoRepository)
        {
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
            var dataVencimentoAnterior = lancamentoEditado.GetDataVencimentoDaParcelaNaSerie();
            var diasDiff = (lancamentoEditado.DataVencimento - dataVencimentoAnterior).TotalDays;

            IEnumerable<Lancamento> lancamentosASeremAlterados;
            if (lancamentoEditado.LancamentoParcelado.TipoDeAlteracaoDaRepeticao == TipoDeAlteracaoDaRepeticaoEnum.AlterarEsteESeguintes)
            {
                SalvarFakesQueNaoSofreramAlteraçãoNaSerie(lancamentoEditado, dataVencimentoAnterior, diasDiff);

                var lancamentoBdAtualMaisSeguintes = GetAllReadOnly().Where(l => l.LancamentoParceladoId == lancamentoEditado.LancamentoParceladoId && l.DataVencimento >= dataVencimentoAnterior).ToList();
                lancamentoBdAtualMaisSeguintes.Add(GetByIdReadOnly(lancamentoEditado.LancamentoParcelado.LancamentoBaseId)); //garante que o lançamento base está sendo adicionado
                AtualizarLancamentos(lancamentoBdAtualMaisSeguintes, lancamentoEditado, diasDiff);
            }
            else
            {
                lancamentosASeremAlterados = GetAllReadOnly().Where(l => l.LancamentoParceladoId == lancamentoEditado.LancamentoParceladoId);
                AtualizarLancamentos(lancamentosASeremAlterados, lancamentoEditado, diasDiff);
            }
        }

        private void AtualizarLancamentos(IEnumerable<Lancamento> lancamentosASeremAlterados, Lancamento lancamentoEditado, double diasDiff)
        {
            foreach (var l in lancamentosASeremAlterados)
            {
                l.Descricao = lancamentoEditado.Descricao;
                l.Valor = lancamentoEditado.Valor;
                l.ContaId = lancamentoEditado.ContaId;
                l.CategoriaId = lancamentoEditado.CategoriaId;
                l.AddDaysDataVencimentoDaParcelaNaSerie(diasDiff);

                if (!l.BaseDaSerie)
                    l.DataVencimento = l.GetDataVencimentoDaParcelaNaSerie(); //.AddDays(diasDiff);
                else
                    l.DataVencimento = l.DataVencimento.AddDays(diasDiff);

                _LancamentoRepository.Update(l);
            }
        }

        /// <summary>
        /// Salva em BD os fakes que não sofreram alterações na série devido ao "Alterar este e seguintes". Atualiza também a data base da série, que é usado como identificado da parcela na série.
        /// </summary>
        private void SalvarFakesQueNaoSofreramAlteraçãoNaSerie(Lancamento lancamentoEditado, DateTime dataVencimentoAnterior, double diasDiff)
        {
            var dataInicio = lancamentoEditado.LancamentoParcelado.DataInicio;
            var lancamentoMaisFake = new LancamentoMaisFakeService(_LancamentoParceladoRepository, _LancamentoRepository);
            var lancamentosMaisFake = lancamentoMaisFake.GetAllMaisFakeAsNoTracking(dataInicio.Month, dataInicio.Year, dataVencimentoAnterior.Month, dataVencimentoAnterior.Year);
            var lancamentosSomenteFake = lancamentosMaisFake.Where(l => l.Fake == true && l.DataVencimento < dataVencimentoAnterior && l.LancamentoParceladoId == lancamentoEditado.LancamentoParceladoId);
            foreach (var lf in lancamentosSomenteFake)
            {
                lf.AddDaysDataVencimentoDaParcelaNaSerie(diasDiff);
                _LancamentoRepository.Add(lf);
            }
        }

        public AgregadoLancamentosDoMes GetLancamentosDoMes(AgregadoLancamentosDoMes lancamentosDoMes)
        {
            var mes = lancamentosDoMes.MesAnoCompetencia.Month;
            var ano = lancamentosDoMes.MesAnoCompetencia.Year;
            var contaId = lancamentosDoMes.ContaIdFiltro;
            var dataUltimoDiaMesAnterior = new DateTime(ano, mes, DateTime.DaysInMonth(ano, mes)).AddMonths(-1);
            var agregadoLancamentosDoMes = new AgregadoLancamentosDoMes();
            
            var lancamentosMesAnteriorTodasAsContas = _LancamentoRepository.GetAll().Where(l => l.DataVencimento <= dataUltimoDiaMesAnterior);

            if (lancamentosDoMes.ContaIdFiltro == Guid.Empty)
                agregadoLancamentosDoMes.SaldoDoMesAnterior = lancamentosMesAnteriorTodasAsContas.Sum(l => l.Valor);
            else
                agregadoLancamentosDoMes.SaldoDoMesAnterior = lancamentosMesAnteriorTodasAsContas.Where(l => l.ContaId == contaId).Sum(l => l.Valor);

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

            return agregadoLancamentosDoMes;
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
        #endregion
    }
}
