namespace Moneta.Infra.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TipoDeTransacao : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Lancamento", "TipoDeTransacao", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Lancamento", "TipoDeTransacao");
        }
    }
}
