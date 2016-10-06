using System;
using Moneta.Domain.Entities;

namespace Moneta.Infra.Data.Migrations
{
    using System.Data.Entity.Migrations;
    using Devart.Data.PostgreSql.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<Context.MonetaContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;

            SetSqlGenerator(PgSqlConnectionInfo.InvariantName, new PgSqlEntityMigrationSqlGenerator());
        }

        protected override void Seed(Context.MonetaContext context)
        {
            //context.Estados.AddOrUpdate(new Estado() { EstadoId = new Guid("D03B8317-9DCD-4C61-81C5-B175C4998FE3"), UF = "AC", Nome = "Acre"} );
            //context.Estados.AddOrUpdate(new Estado() { EstadoId = new Guid("C3980EF8-3301-4EB1-9A23-4B1CEBE72D54"), UF = "AL", Nome = "Alagoas"});        
            //context.Estados.AddOrUpdate(new Estado() { EstadoId = new Guid("B83A4013-975A-4084-B83F-294662154F23"), UF = "AP", Nome = "Amapá"});
        }
    }
}