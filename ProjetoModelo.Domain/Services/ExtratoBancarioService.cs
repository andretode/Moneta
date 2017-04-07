using System;
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

        public ExtratoBancarioService(
            IExtratoBancarioRepository ExtratoBancarioRepository)
            : base(ExtratoBancarioRepository)
        {
            _ExtratoBancarioRepository = ExtratoBancarioRepository;
        }

        public override ExtratoBancario GetById(Guid id)
        {
            return _ExtratoBancarioRepository.GetById(id);
        }

        public override IEnumerable<ExtratoBancario> GetAll()
        {
            return _ExtratoBancarioRepository.GetAll();
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
