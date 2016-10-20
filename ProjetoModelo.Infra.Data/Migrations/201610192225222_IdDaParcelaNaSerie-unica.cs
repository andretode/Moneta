namespace Moneta.Infra.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IdDaParcelaNaSerieunica : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Lancamento", "IdDaParcelaNaSerie", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.Lancamento", new[] { "IdDaParcelaNaSerie" });
        }
    }
}
