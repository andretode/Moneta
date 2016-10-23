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
            var lancamentos = agregadoLancamentosDoMes.LancamentosDoMesPorConta;

            var mes = lancamentosDoMes.MesAnoCompetencia.Month;
            var ano = lancamentosDoMes.MesAnoCompetencia.Year;

            List<DateTime> arrayDataVencimento;
            if(resumido)
                arrayDataVencimento = SomenteDiasDoMesComMovimentacao(mes, ano, lancamentos);
            else
                arrayDataVencimento = TodosDiasDoMes(mes, ano);

            decimal receitaAcumulada = 0;
            decimal despesaAcumulada = 0;
            decimal saldoAcumulado = 0;
            //decimal saldoAcumulado = agregadoLancamentosDoMes.SaldoDoMesAnterior;
            decimal saldoDoMesAnterior = agregadoLancamentosDoMes.SaldoDoMesAnterior;
            if (saldoDoMesAnterior > 0)
                receitaAcumulada = saldoDoMesAnterior;
            else
                despesaAcumulada = saldoDoMesAnterior;
            foreach (var dia in arrayDataVencimento)
            {
                receitaAcumulada += lancamentos.Where(l => l.DataVencimento == dia && l.Valor > 0).Sum(l => l.Valor);
                despesaAcumulada += lancamentos.Where(l => l.DataVencimento == dia && l.Valor < 0).Sum(l => l.Valor);
                //saldoAcumulado += lancamentos.Where(l => l.DataVencimento == dia).Sum(l => l.Valor);
                saldoAcumulado = receitaAcumulada + despesaAcumulada;
                listaDeSaldoPorDia.Add(new Tuple<DateTime, decimal, decimal, decimal>(dia, receitaAcumulada, Math.Abs(despesaAcumulada), saldoAcumulado));
            }

            return listaDeSaldoPorDia;
        }

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
            var dataFinal = new DateTime(ano, mes, DateTime.DaysInMonth(ano,mes));
            var dataCounter = new DateTime(ano,mes,1);
            while(dataCounter <= dataFinal)
            {
                datas.Add(dataCounter);
                dataCounter = dataCounter.AddDays(1);
            }
            return datas;
        }

        public AgregadoLancamentosDoMes GetLancamentosDoMes(AgregadoLancamentosDoMes lancamentosDoMes)
        {
            var mes = lancamentosDoMes.MesAnoCompetencia.Month;
            var ano = lancamentosDoMes.MesAnoCompetencia.Year;
            var contaId = lancamentosDoMes.ContaIdFiltro;
            var dataUltimoDiaMesAnterior = new DateTime(ano, mes, DateTime.DaysInMonth(ano, mes)).AddMonths(-1);
            var agregadoLancamentosDoMes = new AgregadoLancamentosDoMes();
            
            //tratamento dos lançamentos anteriores para calcular o saldo anterior
            var lancamentosMesAnteriorTodasAsContas = _LancamentoRepository.GetAll().Where(l => l.DataVencimento <= dataUltimoDiaMesAnterior && l.Pago == true);
            var saldoMesAnteriorTodasAsContas = lancamentosMesAnteriorTodasAsContas.Sum(l => l.Valor);
            agregadoLancamentosDoMes.SaldoDoMesAnterior = lancamentosMesAnteriorTodasAsContas.Where(l => l.ContaId == contaId).Sum(l => l.Valor);

            var lancamentosDoMesTodasAsContasMaisFake = GetAllMaisFake(mes, ano);
            agregadoLancamentosDoMes.SaldoDoMesTodasAsContas = lancamentosDoMesTodasAsContasMaisFake.Sum(l => l.Valor) 
                + saldoMesAnteriorTodasAsContas;
            agregadoLancamentosDoMes.LancamentosDoMesPorConta = lancamentosDoMesTodasAsContasMaisFake.Where(l => l.ContaId == contaId).OrderBy(l => l.DataVencimento).ThenBy(l => l.DataCadastro);
            agregadoLancamentosDoMes.SaldoDoMesPorConta = agregadoLancamentosDoMes.LancamentosDoMesPorConta.Sum(l => l.Valor) 
                + agregadoLancamentosDoMes.SaldoDoMesAnterior;
            agregadoLancamentosDoMes.SaldoAtualDoMesPorConta = agregadoLancamentosDoMes.LancamentosDoMesPorConta.Where(l => l.Pago == true).Sum(l => l.Valor) 
                + agregadoLancamentosDoMes.SaldoDoMesAnterior;

            if (lancamentosDoMes.PesquisarDescricao != null)
                agregadoLancamentosDoMes.LancamentosDoMesPorConta = agregadoLancamentosDoMes.LancamentosDoMesPorConta.Where(l => 
                    l.Descricao.ToLower().Contains(lancamentosDoMes.PesquisarDescricao.ToLower()) ||
                    l.DataVencimento.ToString("dd/MM/yy").Contains(lancamentosDoMes.PesquisarDescricao.ToLower())
                    );

            return agregadoLancamentosDoMes;
        }

        /// <summary>
        /// Retorna todos os lançamentos do mês mais os fakes
        /// </summary>
        /// <param name="mes">O mês filtro dos lançamentos</param>
        /// <param name="ano">O ano filtro dos lançemtnos</param>
        /// <returns>Lista de lançamentos do mês mais os fakes</returns>
        private List<Lancamento> GetAllMaisFake(int mes, int ano)
        {
            var lancamentosDoMesTodasAsContasComFakes = _LancamentoRepository.GetAll(true).Where(l => l.DataVencimento.Month == mes && l.DataVencimento.Year == ano);
            lancamentosDoMesTodasAsContasComFakes = this.LancamentosFixosFake(mes, ano, lancamentosDoMesTodasAsContasComFakes.ToList());
            lancamentosDoMesTodasAsContasComFakes = lancamentosDoMesTodasAsContasComFakes.Where(l => l.Ativo == true);

            return lancamentosDoMesTodasAsContasComFakes.ToList();
        }

        private List<Lancamento> LancamentosFixosFake(int mes, int ano, List<Lancamento> lancamentosOriginaisMaisOsFakes)
        {
            var mesAnoCompetencia = new DateTime(ano, mes, DateTime.DaysInMonth(ano, mes));
            var lancamentosFixosAptos = _LancamentoParceladoRepository.GetAll().Where(l => l.DataInicio <= mesAnoCompetencia);

            foreach (var lancamentoFixo in lancamentosFixosAptos)
            {
                var lancamentoBase = _LancamentoRepository.GetById(lancamentoFixo.LancamentoBaseId);
                lancamentosOriginaisMaisOsFakes.Remove(lancamentoBase); //Remove ele pois ele não é exibido, serve somente como base para gerar os demais

                switch (lancamentoFixo.Periodicidade)
                {
                    case (int)PeriodicidadeEnum.Semanal:
                        LancamentosFixosSemanais(lancamentosOriginaisMaisOsFakes, lancamentoFixo, lancamentoBase, mes, ano);
                        break;
                    case (int)PeriodicidadeEnum.Mensal:
                        LancamentoFixoMensal(lancamentosOriginaisMaisOsFakes, lancamentoBase, mes, ano);
                        break;
                }
            }

            return lancamentosOriginaisMaisOsFakes;
        }

        private void LancamentosFixosSemanais(List<Lancamento> lancamentosOriginaisMaisOsFakes, 
            LancamentoParcelado lancamentoFixo, Lancamento lancamentoBase, int mes, int ano)
        {
            var diaDaSemanaDoVencimento = lancamentoBase.DataVencimento.DayOfWeek;
            var diaDaSemanaDoPrimeiroDiaDoMes = new DateTime(ano, mes, 1).DayOfWeek;
            var deltaDia = diaDaSemanaDoVencimento - diaDaSemanaDoPrimeiroDiaDoMes + 1;
            deltaDia = (deltaDia < 1 ? deltaDia + 7 : deltaDia);

            var dataVencimento = new DateTime(ano, mes, deltaDia);

            while(dataVencimento.Month == mes)
            {
                Lancamento lancamentoFakeSeguinte = lancamentoBase.CloneFake(dataVencimento);
                InserirFakeApto(lancamentosOriginaisMaisOsFakes, lancamentoFakeSeguinte);

                dataVencimento = dataVencimento.AddDays(7);
            }
        }

        private void LancamentoFixoMensal(List<Lancamento> lancamentosOriginaisMaisOsFakes, Lancamento lancamentoOriginal, int mes, int ano)
        {
            var novoLancamentoFake = lancamentoOriginal.CloneFake(new DateTime(ano, mes, lancamentoOriginal.DataVencimento.Day));
            InserirFakeApto(lancamentosOriginaisMaisOsFakes, novoLancamentoFake);
        }

        private void InserirFakeApto(List<Lancamento> lancamentosOriginaisMaisOsFakes, Lancamento novoFake)
        {
            var lancamentoBd = lancamentosOriginaisMaisOsFakes.Find(l => l.IdDaParcelaNaSerie == novoFake.IdDaParcelaNaSerie);
            if (lancamentoBd == null)
                lancamentosOriginaisMaisOsFakes.Add(novoFake);
        }

    }
}
