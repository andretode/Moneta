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

        public GrupoLancamentoService(
            IGrupoLancamentoRepository GrupoLancamentoRepository)
            : base(GrupoLancamentoRepository)
        {
            _GrupoLancamentoRepository = GrupoLancamentoRepository;
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
