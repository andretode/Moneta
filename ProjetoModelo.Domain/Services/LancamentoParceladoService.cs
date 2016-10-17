﻿using System;
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
    public class LancamentoParceladoService : ServiceBase<LancamentoParcelado>, ILancamentoParceladoService
    {
        private readonly ILancamentoParceladoRepository _LancamentoParceladoRepository;

        public LancamentoParceladoService(
            ILancamentoParceladoRepository LancamentoParceladoRepository)
            : base(LancamentoParceladoRepository)
        {
            _LancamentoParceladoRepository = LancamentoParceladoRepository;
        }

        public override LancamentoParcelado GetById(Guid id)
        {
            return _LancamentoParceladoRepository.GetById(id);
        }

        public LancamentoParcelado GetByIdReadOnly(Guid id)
        {
            return _LancamentoParceladoRepository.GetByIdReadOnly(id);
        }

        public override IEnumerable<LancamentoParcelado> GetAll()
        {
            return _LancamentoParceladoRepository.GetAll();
        }

        public ValidationResult Adicionar(LancamentoParcelado lancamento)
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
    }
}
