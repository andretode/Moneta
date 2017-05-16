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
    public class CategoriaService : ServiceBase<Categoria>, ICategoriaService
    {
        private readonly ICategoriaRepository _categoriaRepository;
        //private readonly ICategoriaReadOnlyRepository _categoriaReadOnlyRepository;
        //private readonly ICategoriaADORepository _categoriaAdoRepository;

        public CategoriaService(
            ICategoriaRepository categoriaRepository)
            //ICategoriaReadOnlyRepository categoriaReadOnlyRepository, 
            //ICategoriaADORepository categoriaAdoRepository)
            : base(categoriaRepository)
        {
            _categoriaRepository = categoriaRepository;
            //_categoriaReadOnlyRepository = categoriaReadOnlyRepository;
            //_categoriaAdoRepository = categoriaAdoRepository;
        }

        public override Categoria GetById(Guid id)
        {
            return _categoriaRepository.GetById(id);
            //return _categoriaReadOnlyRepository.GetById(id);
        }

        public override IEnumerable<Categoria> GetAll()
        {
            return _categoriaRepository.GetAll().OrderBy(c => c.Descricao);
            //return _categoriaReadOnlyRepository.GetAll();
        }

        public ValidationResult Adicionar(Categoria categoria)
        {
            var resultadoValidacao = new ValidationResult();

            if (!categoria.IsValid())
            {
                resultadoValidacao.AdicionarErro(categoria.ResultadoValidacao);
                return resultadoValidacao;
            }

            base.Add(categoria);

            return resultadoValidacao;
        }

        //public IEnumerable<Categoria> ObterCategoriasGrid(int page, string pesquisa)
        //{
        //    return _categoriaReadOnlyRepository.ObterCategoriasGrid(page, pesquisa);
        //}


        //public int ObterTotalRegistros(string pesquisa)
        //{
        //    return _categoriaReadOnlyRepository.ObterTotalRegistros(pesquisa);
        //}
    }
}
