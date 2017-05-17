using System;
using System.Linq;
using System.Collections.Generic;
using Moneta.Domain.Entities;
using Moneta.Domain.Interfaces.Repository;
using Moneta.Domain.Interfaces.Services;
using Moneta.Domain.ValueObjects;

namespace Moneta.Domain.Services
{
    public class GrupoLancamentoService : ServiceBase<GrupoLancamento>, IGrupoLancamentoService
    {
        private readonly IGrupoLancamentoRepository _GrupoLancamentoRepository;
        private readonly ILancamentoRepository _LancamentoRepository;

        public GrupoLancamentoService(
            IGrupoLancamentoRepository GrupoLancamentoRepository,
            ILancamentoRepository LancamentoRepository)
            : base(GrupoLancamentoRepository)
        {
            _GrupoLancamentoRepository = GrupoLancamentoRepository;
            _LancamentoRepository = LancamentoRepository;
        }

        public override GrupoLancamento GetById(Guid id)
        {
            var grupo = _GrupoLancamentoRepository.GetById(id);
            grupo.GruposDeLancamentos = _GrupoLancamentoRepository.GetAll().Where(g => g.GrupoLancamentoIdPai == id).ToList();
            return grupo;
        }

        public GrupoLancamento GetByIdReadOnly(Guid id)
        {
            return _GrupoLancamentoRepository.GetByIdReadOnly(id);
        }

        public override IEnumerable<GrupoLancamento> GetAll()
        {
            return _GrupoLancamentoRepository.GetAll();
        }

        public override void Remove(GrupoLancamento grupoLancamento)
        {
            foreach (var lancamento in _LancamentoRepository.GetAllReadOnly().Where(l => l.GrupoLancamentoId == grupoLancamento.GrupoLancamentoId))
                _LancamentoRepository.Remove(lancamento);

            _GrupoLancamentoRepository.Remove(grupoLancamento);
        }

        public ValidationResult Adicionar(GrupoLancamento GrupoLancamento)
        {
            var resultadoValidacao = new ValidationResult();

            if (!GrupoLancamento.IsValid())
            {
                resultadoValidacao.AdicionarErro(GrupoLancamento.ResultadoValidacao);
                return resultadoValidacao;
            }

            base.Add(GrupoLancamento);

            return resultadoValidacao;
        }
    }
}
