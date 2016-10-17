namespace Moneta.Infra.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NumeroParcelasOpcinal : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.LancamentoParcelado", "NumeroParcelas", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.LancamentoParcelado", "NumeroParcelas", c => c.Int(nullable: false));
        }
    }
}
