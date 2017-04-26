using System;
using System.Linq;
using System.Collections.Generic;
using Moneta.Domain.Entities;
using Moneta.Domain.Interfaces.Repository;
using Moneta.Domain.Interfaces.Services;
using Moneta.Domain.ValueObjects;

namespace Moneta.Domain.Services
{
    public class ExtratoBancarioService : ServiceBase<ExtratoBancario>, IExtratoBancarioService
    {
        private readonly IExtratoBancarioRepository _ExtratoBancarioRepository;
        private readonly ILancamentoRepository _LancamentoRepository;
        private readonly IGrupoLancamentoRepository _GrupoLancamentoRepository;

        public ExtratoBancarioService(
            IExtratoBancarioRepository ExtratoBancarioRepository,
            ILancamentoRepository LancamentoRepository,
            IGrupoLancamentoRepository GrupoLancamentoRepository)
            : base(ExtratoBancarioRepository)
        {
            _ExtratoBancarioRepository = ExtratoBancarioRepository;
            _LancamentoRepository = LancamentoRepository;
            _GrupoLancamentoRepository = GrupoLancamentoRepository;
        }

        public IEnumerable<ExtratoBancario> GetExtratosDoMes(DateTime mesAnoCompetencia, Guid contaId)
        {
            var extratos = _ExtratoBancarioRepository.GetAll()
                .Where(e => e.DataCompensacao.Month == mesAnoCompetencia.Month && e.DataCompensacao.Year == mesAnoCompetencia.Year);

            if (contaId != Guid.Empty)
                extratos = extratos.Where(e => e.ContaId == contaId);

            extratos = extratos.OrderBy(e => e.DataCompensacao).OrderBy(e => e.Descricao);

            foreach (var extr in extratos)
            {
                extr.Lancamento = _LancamentoRepository.GetAll().Where(l => l.ExtratoBancarioId == extr.ExtratoBancarioId).SingleOrDefault();
                extr.GrupoLancamento = _GrupoLancamentoRepository.GetAll().Where(g => g.ExtratoBancarioId == extr.ExtratoBancarioId).SingleOrDefault();
            }

            return extratos;
        }

        public ValidationResult Adicionar(ExtratoBancario ExtratoBancario)
        {
            var resultadoValidacao = new ValidationResult();

            if (!ExtratoBancario.IsValid())
            {
                resultadoValidacao.AdicionarErro(ExtratoBancario.ResultadoValidacao);
                return resultadoValidacao;
            }

            base.Add(ExtratoBancario);

            return resultadoValidacao;
        }

        public int ImportarOfx(string caminhoOfx, Guid contaId)
        {
            return _ExtratoBancarioRepository.ImportarOfx(caminhoOfx, contaId);
        }

        public void RemoveAll(IEnumerable<ExtratoBancario> extratos)
        {
            foreach(var extrato in extratos)
                _ExtratoBancarioRepository.Remove(extrato);
        }
    }
}
