namespace Moneta.Infra.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Categoria_OrcamentoMensal : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Categoria", "OrcamentoMensal", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Categoria", "OrcamentoMensal");
        }
    }
}
