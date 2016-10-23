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
        /// Retorna todos os lançamentos do mês mais os fakes
        /// </summary>
        /// <param name="mes">O mês filtro dos lançamentos</param>
        /// <param name="ano">O ano filtro dos lançemtnos</param>
        /// <returns>Lista de lançamentos do mês mais os fakes</returns>
        public List<Lancamento> GetAllMaisFake(int mes, int ano)
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

            while (dataVencimento.Month == mes)
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
