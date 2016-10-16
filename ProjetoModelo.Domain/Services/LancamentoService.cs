using System;
using System.Collections.Generic;
using System.Linq;
using Moneta.Domain.Entities;
using Moneta.Domain.Interfaces.Repository;
using Moneta.Domain.Interfaces.Repository.ADO;
using Moneta.Domain.Interfaces.Repository.ReadOnly;
using Moneta.Domain.Interfaces.Services;
using Moneta.Domain.ValueObjects;

namespace Moneta.Domain.Services
{
    public class LancamentoService : ServiceBase<Lancamento>, ILancamentoService
    {
        private readonly ILancamentoRepository _LancamentoRepository;

        public LancamentoService(
            ILancamentoRepository LancamentoRepository)
            : base(LancamentoRepository)
        {
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
            lancamentosDoMesPorConta.LancamentosDoMesPorConta = lancamentosDoMesTodasAsContas.Where(l => l.ContaId == contaId).OrderBy(l => l.DataVencimento).ThenBy(l => l.Descricao);
            lancamentosDoMesPorConta.SaldoDoMesPorConta = lancamentosDoMesPorConta.LancamentosDoMesPorConta.Sum(l => l.Valor) 
                + lancamentosDoMesPorConta.SaldoDoMesAnterior;
            lancamentosDoMesPorConta.SaldoAtualDoMesPorConta = lancamentosDoMesPorConta.LancamentosDoMesPorConta.Where(l => l.Pago == true).Sum(l => l.Valor) 
                + lancamentosDoMesPorConta.SaldoDoMesAnterior;

            if (lancamentosDoMes.PesquisarDescricao != null)
                lancamentosDoMesPorConta.LancamentosDoMesPorConta = lancamentosDoMesPorConta.LancamentosDoMesPorConta.Where(l => 
                    l.Descricao.ToLower().Contains(lancamentosDoMes.PesquisarDescricao.ToLower()));

            return lancamentosDoMesPorConta;
        }
    }
}
