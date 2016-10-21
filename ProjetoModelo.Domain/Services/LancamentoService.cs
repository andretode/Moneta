﻿using System;
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

        public override IEnumerable<Lancamento> GetAll()
        {
            return _LancamentoRepository.GetAll();
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

        public List<Tuple<DateTime, decimal>> GetSaldoDoMesPorDia(AgregadoLancamentosDoMes lancamentosDoMes)
        {
            var listaDeSaldoPorDia = new List<Tuple<DateTime, decimal>>();
            var lancamentos = GetLancamentosDoMes(lancamentosDoMes).LancamentosDoMesPorConta;
            var arrayDataVencimento = lancamentos.Select(l => l.DataVencimento).ToArray().Distinct();
            decimal saldoAcumulado = lancamentosDoMes.SaldoDoMesAnterior;
            foreach (var dia in arrayDataVencimento)
            {
                saldoAcumulado += lancamentos.Where(l => l.DataVencimento == dia).Sum(l => l.Valor);
                listaDeSaldoPorDia.Add(new Tuple<DateTime, decimal>(dia, saldoAcumulado));
            }

            return listaDeSaldoPorDia;
        }

        public AgregadoLancamentosDoMes GetLancamentosDoMes(AgregadoLancamentosDoMes lancamentosDoMes)
        {
            var mes = lancamentosDoMes.MesAnoCompetencia.Month;
            var ano = lancamentosDoMes.MesAnoCompetencia.Year;
            var contaId = lancamentosDoMes.ContaIdFiltro;
            var dataUltimoDiaMesAnterior = new DateTime(ano, mes, DateTime.DaysInMonth(ano, mes)).AddMonths(-1);
            var agregadoLancamentosDoMes = new AgregadoLancamentosDoMes();
            var lancamentosDoMesTodasAsContasComFakes = _LancamentoRepository.GetAll(true).Where(l => l.DataVencimento.Month == mes && l.DataVencimento.Year == ano);

            //inclusao dos fakes
            lancamentosDoMesTodasAsContasComFakes = this.LancamentosFixosFake(mes, ano, lancamentosDoMesTodasAsContasComFakes.ToList());
            lancamentosDoMesTodasAsContasComFakes = lancamentosDoMesTodasAsContasComFakes.Where(l => l.Ativo == true);

            //CONFIRMAR, POIS CREIO QUE TENHA QUE PEGAR SOMENTE OS "PAGOS"
            var saldoMesAnteriorTodasAsContas = _LancamentoRepository.GetAll().Where(l => l.DataVencimento <= dataUltimoDiaMesAnterior && l.Pago == true).Sum(l => l.Valor);
            agregadoLancamentosDoMes.SaldoDoMesAnterior = _LancamentoRepository.GetAll().Where(l => l.DataVencimento <= dataUltimoDiaMesAnterior && l.ContaId == contaId && l.Pago == true).Sum(l => l.Valor);

            agregadoLancamentosDoMes.SaldoDoMesTodasAsContas = lancamentosDoMesTodasAsContasComFakes.Sum(l => l.Valor) 
                + saldoMesAnteriorTodasAsContas;
            agregadoLancamentosDoMes.LancamentosDoMesPorConta = lancamentosDoMesTodasAsContasComFakes.Where(l => l.ContaId == contaId).OrderBy(l => l.DataVencimento).ThenBy(l => l.DataCadastro);
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
