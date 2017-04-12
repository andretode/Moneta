﻿using System;
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

        public ExtratoBancarioService(
            IExtratoBancarioRepository ExtratoBancarioRepository,
            ILancamentoRepository LancamentoRepository)
            : base(ExtratoBancarioRepository)
        {
            _ExtratoBancarioRepository = ExtratoBancarioRepository;
            _LancamentoRepository = LancamentoRepository;
        }

        //public override ExtratoBancario GetById(Guid id)
        //{
        //    return _ExtratoBancarioRepository.GetById(id);
        //}

        public override IEnumerable<ExtratoBancario> GetAll()
        {
            var extratos = _ExtratoBancarioRepository.GetAll();

            foreach(var extr in extratos)
                extr.Lancamento = _LancamentoRepository.GetAll().Where(l => l.ExtratoBancarioId == extr.ExtratoBancarioId).SingleOrDefault();

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

        public void ImportarOfx(string caminhoOfx, Guid contaId)
        {
            _ExtratoBancarioRepository.ImportarOfx(caminhoOfx, contaId);
        }

        public void RemoveAll(IEnumerable<ExtratoBancario> extratos)
        {
            foreach(var extrato in extratos)
                _ExtratoBancarioRepository.Remove(extrato);
        }
    }
}
