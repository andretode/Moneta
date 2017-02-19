using System;
using System.Data.Entity;
using System.Linq;
using System.Data.Entity.Migrations;
using MySql.Data.Entity;
using Moneta.Domain.Entities;

namespace Moneta.Infra.Data.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<Moneta.Infra.Data.Context.MonetaContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;

            SetSqlGenerator(MySqlProviderInvariantName.ProviderName, new MySqlMigrationSqlGenerator());
        }

        protected override void Seed(Moneta.Infra.Data.Context.MonetaContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
            
            context.Categorias.AddOrUpdate(
                new Categoria { Descricao = "Moradia", Cor = "#2655cc" },
                new Categoria { Descricao = "Transporte", Cor = "#199982" },
                new Categoria { Descricao = "Lazer", Cor = "#f7ed00" },
                new Categoria { Descricao = "Outros", Cor = "#777777" }
            );
        }
    }
}
