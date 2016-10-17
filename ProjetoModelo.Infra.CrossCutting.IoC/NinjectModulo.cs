using Ninject.Modules;
using Moneta.Application;
using Moneta.Application.Interfaces;
using Moneta.Domain.Interfaces.Repository;
using Moneta.Domain.Interfaces.Repository.ADO;
using Moneta.Domain.Interfaces.Repository.ReadOnly;
using Moneta.Domain.Interfaces.Services;
using Moneta.Domain.Services;
using Moneta.Infra.Data.Context;
using Moneta.Infra.Data.Interfaces;
using Moneta.Infra.Data.Repositories;
using Moneta.Infra.Data.Repositories.ADO;
using Moneta.Infra.Data.Repositories.ReadOnly;
using Moneta.Infra.Data.UoW;

namespace Moneta.Infra.CrossCutting.IoC
{
    public class NinjectModulo : NinjectModule
    {
        public override void Load()
        {
            // app
            Bind<IContaAppService>().To<ContaAppService>();
            Bind<ICategoriaAppService>().To<CategoriaAppService>();
            Bind<ILancamentoAppService>().To<LancamentoAppService>();
            Bind<ILancamentoParceladoAppService>().To<LancamentoParceladoAppService>();

            // service
            Bind(typeof(IServiceBase<>)).To(typeof(ServiceBase<>));
            Bind<IContaService>().To<ContaService>();
            Bind<ICategoriaService>().To<CategoriaService>();
            Bind<ILancamentoService>().To<LancamentoService>();
            Bind<ILancamentoParceladoService>().To<LancamentoParceladoService>();

            // data repos
            Bind(typeof(IRepositoryBase<>)).To(typeof(RepositoryBase<,>));
            Bind<IContaRepository>().To<ContaRepository>();
            Bind<ICategoriaRepository>().To<CategoriaRepository>();
            Bind<ILancamentoRepository>().To<LancamentoRepository>();
            Bind<ILancamentoParceladoRepository>().To<LancamentoParceladoRepository>();
      
            // data repos read only
            Bind<IContaReadOnlyRepository>().To<ContaReadOnlyRepository>();

            // ado repos only
            Bind<IContaADORepository>().To<ContaADORepository>();

            // data configs
            Bind(typeof(IContextManager<>)).To(typeof(ContextManager<>));
            Bind<IDbContext>().To<MonetaContext>();
            Bind(typeof(IUnitOfWork<>)).To(typeof(UnitOfWork<>));

        }
    }
}
