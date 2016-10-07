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
    public class ContaService : ServiceBase<Conta>, IContaService
    {
        private readonly IContaRepository _contaRepository;
        private readonly IContaReadOnlyRepository _contaReadOnlyRepository;
        private readonly IContaADORepository _contaAdoRepository;

        public ContaService(
            IContaRepository contaRepository, 
            IContaReadOnlyRepository contaReadOnlyRepository, 
            IContaADORepository contaAdoRepository)
            : base(contaRepository)
        {
            _contaRepository = contaRepository;
            _contaReadOnlyRepository = contaReadOnlyRepository;
            _contaAdoRepository = contaAdoRepository;
        }

        public override Conta GetById(Guid id)
        {
            return _contaReadOnlyRepository.GetById(id);
        }

        public override IEnumerable<Conta> GetAll()
        {
            return _contaReadOnlyRepository.GetAll();
        }

        public ValidationResult Adicionar(Conta conta)
        {
            var resultadoValidacao = new ValidationResult();

            if (!conta.IsValid())
            {
                resultadoValidacao.AdicionarErro(conta.ResultadoValidacao);
                return resultadoValidacao;
            }

            base.Add(conta);

            return resultadoValidacao;
        }

        public IEnumerable<Conta> ObterContasGrid(int page, string pesquisa)
        {
            return _contaReadOnlyRepository.ObterContasGrid(page, pesquisa);
        }


        public int ObterTotalRegistros(string pesquisa)
        {
            return _contaReadOnlyRepository.ObterTotalRegistros(pesquisa);
        }
    }
}
