namespace Moneta.Infra.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BaseDaSerie : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Lancamento", "BaseDaSerie", c => c.Boolean(nullable: false));
            AddColumn("dbo.LancamentoParcelado", "LancamentoBaseId", c => c.Guid(nullable: false));
            DropColumn("dbo.LancamentoParcelado", "LancamentoOriginalId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.LancamentoParcelado", "LancamentoOriginalId", c => c.Guid(nullable: false));
            DropColumn("dbo.LancamentoParcelado", "LancamentoBaseId");
            DropColumn("dbo.Lancamento", "BaseDaSerie");
        }
    }
}
