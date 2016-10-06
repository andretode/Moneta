using System;
using System.Data.Entity;
using System.Linq;
using System.Data.Entity.Migrations;
using Devart.Data.PostgreSql.Entity.Migrations;

namespace Moneta.Infra.Data.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<Moneta.Infra.Data.Context.MonetaContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;

            SetSqlGenerator(PgSqlConnectionInfo.InvariantName, new PgSqlEntityMigrationSqlGenerator());
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
        }
    }
}
