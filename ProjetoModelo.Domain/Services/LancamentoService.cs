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

        public LancamentosDoMes GetLancamentosDoMes(int mes, Guid contaId)
        {
            var lancamentosDoMesPorConta = new LancamentosDoMes();
            var lancamentosDoMes = this.GetAll().Where(l => l.DataVencimento.Month == mes);
            lancamentosDoMesPorConta.SaldoDoMesTodasAsContas = lancamentosDoMes.Sum(l => l.Valor);
            lancamentosDoMesPorConta.LancamentosDoMesPorConta = lancamentosDoMes.Where(l => l.ContaId == contaId).OrderBy(l => l.DataVencimento).ThenBy(l => l.Descricao);
            lancamentosDoMesPorConta.SaldoDoMesPorConta = lancamentosDoMesPorConta.LancamentosDoMesPorConta.Sum(l => l.Valor);
            lancamentosDoMesPorConta.SaldoAtualDoMesPorConta = lancamentosDoMesPorConta.LancamentosDoMesPorConta.Where(l => l.Pago == true).Sum(l => l.Valor);

            return lancamentosDoMesPorConta;
        }
    }
}
