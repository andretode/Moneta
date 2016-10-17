namespace Moneta.Infra.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Periodicidade : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LancamentoParcelado", "Periodicidade", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.LancamentoParcelado", "Periodicidade");
        }
    }
}
