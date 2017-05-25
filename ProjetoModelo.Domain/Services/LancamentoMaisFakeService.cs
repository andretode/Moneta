using Moneta.Domain.Entities;
using Moneta.Domain.Interfaces.Repository;
using Moneta.Infra.CrossCutting.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Moneta.Domain.Services
{
    public class LancamentoMaisFakeService
    {
        private readonly ILancamentoRepository _LancamentoRepository;
        private readonly ILancamentoParceladoRepository _LancamentoParceladoRepository;

        public LancamentoMaisFakeService(
            ILancamentoParceladoRepository LancamentoParceladoRepository,
            ILancamentoRepository LancamentoRepository)
        {
            _LancamentoParceladoRepository = LancamentoParceladoRepository;
            _LancamentoRepository = LancamentoRepository;
        }

        /// <summary>
        /// Retorna os lançamentos fakes e não fakes de um período em meses.
        /// </summary>
        public List<Lancamento> GetAllMaisFake(int mesInicio, int anoInicio, int mesFim, int anoFim, bool asNoTracking = false)
        {
            var lancamentos = new List<Lancamento>();

            //Cria o intervalo. O dia não importa, umas vez que estamos filtrando meses.
            var dataCounter = new DateTime(anoFim, mesFim, 1);
            var dataInicio = new DateTime(anoInicio, mesInicio, 1);

            while (dataCounter >= dataInicio)
            {
                lancamentos.AddRange(GetAllMaisFake(dataCounter.Month, dataCounter.Year, asNoTracking));
                dataCounter = dataCounter.AddMonths(-1);
            }

            return lancamentos;
        }

        /// <summary>
        /// Retorna os lançamentos fakes e não fakes de um período em meses.
        /// </summary>
        public List<Lancamento> GetAllMaisFakeAsNoTracking(int mesInicio, int anoInicio, int mesFim, int anoFim)
        {
            return GetAllMaisFake(mesInicio, anoInicio, mesFim, anoFim, true);
        }

        /// <summary>
        /// Retorna todos os lançamentos do mês mais os fakes
        /// </summary>
        /// <param name="mes">O mês filtro dos lançamentos</param>
        /// <param name="ano">O ano filtro dos lançemtnos</param>
        /// <returns>Lista de lançamentos do mês mais os fakes</returns>
        public List<Lancamento> GetAllMaisFake(int mes, int ano, bool asNoTracking = false)
        {
            var lancamentosDoMesTodasAsContasComFakes = _LancamentoRepository.GetAll(false, asNoTracking).Where(l => l.DataVencimento.Month == mes && l.DataVencimento.Year == ano);
            lancamentosDoMesTodasAsContasComFakes = this.LancamentosFixosFake(mes, ano, lancamentosDoMesTodasAsContasComFakes.ToList(), asNoTracking);
            lancamentosDoMesTodasAsContasComFakes = lancamentosDoMesTodasAsContasComFakes.Where(l => l.Ativo == true);

            return lancamentosDoMesTodasAsContasComFakes.ToList();
        }

        private List<Lancamento> LancamentosFixosFake(int mes, int ano, List<Lancamento> lancamentosOriginaisMaisOsFakes, bool asNoTracking)
        {
            var mesAnoCompetencia = new DateTime(ano, mes, DateTime.DaysInMonth(ano, mes));
            var lancamentosFixosAptos = _LancamentoParceladoRepository.GetAll().Where(l => l.DataInicio <= mesAnoCompetencia && l.NumeroParcelas==null);

            foreach (var lancamentoFixo in lancamentosFixosAptos)
            {
                Lancamento lancamentoBase;
                if(asNoTracking)
                    lancamentoBase = _LancamentoRepository.GetByIdReadOnly(lancamentoFixo.LancamentoBaseId);
                else
                    lancamentoBase = _LancamentoRepository.GetById(lancamentoFixo.LancamentoBaseId);

                if (lancamentoBase == null || !lancamentoBase.Ativo)
                {
                    //Remove o LancamentoParcelado uma vez que ele não é mais necessário devido a base ter sido removida.
                    _LancamentoParceladoRepository.BeginTransaction();
                    _LancamentoParceladoRepository.Remove(lancamentoFixo);
                    _LancamentoParceladoRepository.Commit();
                }
                else
                {
                    lancamentosOriginaisMaisOsFakes.Remove(lancamentoBase); //Remove ele pois ele não é exibido, serve somente como base para gerar os demais

                    switch (lancamentoFixo.Periodicidade)
                    {
                        case (int)PeriodicidadeEnum.Diario:
                            LancamentosFixosDentroDoMes((int)PeriodicidadeEnum.Diario, lancamentosOriginaisMaisOsFakes, lancamentoFixo, lancamentoBase, mes, ano);
                            break;
                        case (int)PeriodicidadeEnum.Semanal:
                            LancamentosFixosDentroDoMes((int)PeriodicidadeEnum.Semanal, lancamentosOriginaisMaisOsFakes, lancamentoFixo, lancamentoBase, mes, ano);
                            break;
                        case (int)PeriodicidadeEnum.Quinzenal:
                            LancamentosFixosDentroDoMes((int)PeriodicidadeEnum.Quinzenal, lancamentosOriginaisMaisOsFakes, lancamentoFixo, lancamentoBase, mes, ano);
                            break;
                        case (int)PeriodicidadeEnum.Mensal:
                            LancamentoFixoNoMensal(lancamentosOriginaisMaisOsFakes, lancamentoBase, mes, ano);
                            break;
                        case (int)PeriodicidadeEnum.Trimestral:
                            LancamentoFixoForaDoMensal(3, lancamentosOriginaisMaisOsFakes, lancamentoBase, mes, ano);
                            break;
                        case (int)PeriodicidadeEnum.Semestral:
                            LancamentoFixoForaDoMensal(6, lancamentosOriginaisMaisOsFakes, lancamentoBase, mes, ano);
                            break;
                        case (int)PeriodicidadeEnum.Anual:
                            LancamentoFixoForaDoMensal(12, lancamentosOriginaisMaisOsFakes, lancamentoBase, mes, ano);
                            break;
                    }
                }
            }

            return lancamentosOriginaisMaisOsFakes;
        }

        private void LancamentosFixosDentroDoMes(int periodicidadeEmDias, List<Lancamento> lancamentosOriginaisMaisOsFakes,
            LancamentoParcelado lancamentoFixo, Lancamento lancamentoBase, int mes, int ano)
        {
            if(lancamentoBase != null)
            {
                var diaDaSemanaDoVencimento = lancamentoBase.DataVencimento.DayOfWeek;
                var diaDaSemanaDoPrimeiroDiaDoMes = new DateTime(ano, mes, 1).DayOfWeek;
                var deltaDia = diaDaSemanaDoVencimento - diaDaSemanaDoPrimeiroDiaDoMes + 1;
                deltaDia = (deltaDia < 1 ? deltaDia + periodicidadeEmDias : deltaDia);

                var dataVencimentoFake = new DateTime(ano, mes, deltaDia);
                if (dataVencimentoFake < lancamentoBase.DataVencimento)
                    dataVencimentoFake = lancamentoBase.DataVencimento;

                while (dataVencimentoFake.Month == mes)
                {
                    Lancamento lancamentoFakeSeguinte = lancamentoBase.CloneFake(dataVencimentoFake);
                    InserirFakeApto(lancamentosOriginaisMaisOsFakes, lancamentoFakeSeguinte);

                    dataVencimentoFake = dataVencimentoFake.AddDays(periodicidadeEmDias);
                }
            }
        }

        private void LancamentoFixoForaDoMensal(int perioricidadeEmMeses, List<Lancamento> lancamentosOriginaisMaisOsFakes, Lancamento lancamentoOriginal, int mes, int ano)
        {
            if(IsMesConsultadoDentroDaPeriodicidade(perioricidadeEmMeses, lancamentoOriginal, mes, ano))
            {
                LancamentoFixoNoMensal(lancamentosOriginaisMaisOsFakes, lancamentoOriginal, mes, ano);
            }
        }

        private void LancamentoFixoNoMensal(List<Lancamento> lancamentosOriginaisMaisOsFakes, Lancamento lancamentoOriginal, int mes, int ano)
        {
            var dia = GaratirQueDiaEstejaNoMes(ano, mes, lancamentoOriginal.DataVencimento.Day);
            var novoLancamentoFake = lancamentoOriginal.CloneFake(new DateTime(ano, mes, dia));
            InserirFakeApto(lancamentosOriginaisMaisOsFakes, novoLancamentoFake);
        }

        private bool IsMesConsultadoDentroDaPeriodicidade(int perioricidadeEmMeses, Lancamento lancamentoOriginal, int mes, int ano)
        {
            var dataMesConsultado = new DateTime(ano, mes, lancamentoOriginal.DataVencimento.Day);
            var diffEmMeses = DiffEmMeses(lancamentoOriginal.DataVencimento, dataMesConsultado);
            return (diffEmMeses % perioricidadeEmMeses) == 0;
        }

        private int DiffEmMeses(DateTime dataInicio, DateTime dataFim)
        {
            var anoDelta = dataFim.Year - dataInicio.Year;
            var mesDelta = dataFim.Month - dataInicio.Month;

            return mesDelta + (anoDelta * 12);
        }

        private void InserirFakeApto(List<Lancamento> lancamentosOriginaisMaisOsFakes, Lancamento novoFake)
        {
            var lancamentoBd = lancamentosOriginaisMaisOsFakes.Find(l => l.IdDaParcelaNaSerie == novoFake.IdDaParcelaNaSerie);
            if (lancamentoBd == null)
                lancamentosOriginaisMaisOsFakes.Add(novoFake);
        }

        private int GaratirQueDiaEstejaNoMes(int ano, int mes, int dia)
        {
            var ultimoDiaDoMes = DateTime.DaysInMonth(ano, mes);
            return (dia > ultimoDiaDoMes ? ultimoDiaDoMes : dia);
        }
    }
}
