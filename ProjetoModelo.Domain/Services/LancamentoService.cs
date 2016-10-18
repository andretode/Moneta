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

        public LancamentosDoMes GetLancamentosDoMes(LancamentosDoMes lancamentosDoMes)
        {
            var mes = lancamentosDoMes.MesAnoCompetencia.Month;
            var ano = lancamentosDoMes.MesAnoCompetencia.Year;
            var contaId = lancamentosDoMes.ContaIdFiltro;
            var dataUltimoDiaMesAnterior = new DateTime(ano, mes, DateTime.DaysInMonth(ano, mes)).AddMonths(-1);
            var lancamentosDoMesPorConta = new LancamentosDoMes();
            var lancamentosDoMesTodasAsContas = this.GetAll().Where(l => l.DataVencimento.Month == mes && l.DataVencimento.Year == ano);
            var saldoMesAnteriorTodasAsContas = this.GetAll().Where(l => l.DataVencimento <= dataUltimoDiaMesAnterior).Sum(l => l.Valor);
            lancamentosDoMesPorConta.SaldoDoMesAnterior = this.GetAll().Where(l => l.DataVencimento <= dataUltimoDiaMesAnterior && l.ContaId == contaId).Sum(l => l.Valor);
            lancamentosDoMesPorConta.SaldoDoMesTodasAsContas = lancamentosDoMesTodasAsContas.Sum(l => l.Valor) 
                + saldoMesAnteriorTodasAsContas;
            lancamentosDoMesPorConta.LancamentosDoMesPorConta = lancamentosDoMesTodasAsContas.Where(l => l.ContaId == contaId);
            var lacamentosOriginaisMaisOsFakes = lancamentosDoMesPorConta.LancamentosDoMesPorConta.ToList();
            lacamentosOriginaisMaisOsFakes.AddRange(this.LancamentosFixosFake(mes, ano));
            lancamentosDoMesPorConta.LancamentosDoMesPorConta = lacamentosOriginaisMaisOsFakes.OrderBy(l => l.DataVencimento).ThenBy(l => l.DataCadastro);
            lancamentosDoMesPorConta.SaldoDoMesPorConta = lancamentosDoMesPorConta.LancamentosDoMesPorConta.Sum(l => l.Valor) 
                + lancamentosDoMesPorConta.SaldoDoMesAnterior;
            lancamentosDoMesPorConta.SaldoAtualDoMesPorConta = lancamentosDoMesPorConta.LancamentosDoMesPorConta.Where(l => l.Pago == true).Sum(l => l.Valor) 
                + lancamentosDoMesPorConta.SaldoDoMesAnterior;

            if (lancamentosDoMes.PesquisarDescricao != null)
                lancamentosDoMesPorConta.LancamentosDoMesPorConta = lancamentosDoMesPorConta.LancamentosDoMesPorConta.Where(l => 
                    l.Descricao.ToLower().Contains(lancamentosDoMes.PesquisarDescricao.ToLower()) ||
                    l.DataVencimento.ToString("dd/MM/yy").Contains(lancamentosDoMes.PesquisarDescricao.ToLower())
                    );

            return lancamentosDoMesPorConta;
        }

        private List<Lancamento> LancamentosFixosFake(int mes, int ano)
        {
            var lancamentosFixos = new List<Lancamento>();

            var mesAnoCompetencia = new DateTime(ano, mes, DateTime.DaysInMonth(ano, mes));
            var lancamentosFixosAptos = _LancamentoParceladoRepository.GetAll().Where(l => l.DataInicio < mesAnoCompetencia);

            foreach (var lancamentoFixo in lancamentosFixosAptos)
            {
                var lancamentoOrigem = _LancamentoRepository.Find(l => l.LancamentoParceladoId == lancamentoFixo.LancamentoParceladoId).FirstOrDefault();

                switch (lancamentoFixo.Periodicidade)
                {
                    case (int)PeriodicidadeEnum.Semanal:
                        LancamentosFixosSemanais(lancamentosFixos, lancamentoFixo, lancamentoOrigem, mes, ano);
                        break;
                    case (int)PeriodicidadeEnum.Mensal:
                        LancamentoFixoMensal(lancamentosFixos, lancamentoOrigem, mes, ano);
                        break;
                }
            }

            return lancamentosFixos;
        }

        private void LancamentosFixosSemanais(List<Lancamento> lancamentosFixos, LancamentoParcelado lancamentoFixo, Lancamento lancamentoOrigem, int mes, int ano)
        {
            var diaDaSemanaDoVencimento = lancamentoOrigem.DataVencimento.DayOfWeek;
            var diaDaSemanaDoPrimeiroDiaDoMes = new DateTime(ano, mes, 1).DayOfWeek;
            var deltaDia = diaDaSemanaDoVencimento - diaDaSemanaDoPrimeiroDiaDoMes + 1;
            deltaDia = (deltaDia < 1 ? deltaDia + 7 : deltaDia);

            var dataVencimento = new DateTime(ano, mes, deltaDia);

            while(dataVencimento.Month == mes)
            {
                Lancamento lancamentoFakeSeguinte = lancamentoOrigem.Clone();
                lancamentoFakeSeguinte.Descricao += " (semanal)";
                lancamentoFakeSeguinte.DataVencimento = dataVencimento;

                if (lancamentoFakeSeguinte.DataVencimento != lancamentoOrigem.DataVencimento)
                    lancamentosFixos.Add(lancamentoFakeSeguinte);

                dataVencimento = dataVencimento.AddDays(7);
            }
        }

        private void LancamentoFixoMensal(List<Lancamento> lancamentosFixos, Lancamento lancamentoOrigem, int mes, int ano)
        {
            if (lancamentoOrigem.DataVencimento != new DateTime(ano, mes, lancamentoOrigem.DataVencimento.Day))
            {
                var novoLancamentoFake = lancamentoOrigem.Clone();
                novoLancamentoFake.Descricao += " (mensal)";
                novoLancamentoFake.DataVencimento = new DateTime(ano, mes, lancamentoOrigem.DataVencimento.Day);
                lancamentosFixos.Add(novoLancamentoFake);
            }
        }

    }
}
