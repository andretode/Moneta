namespace Moneta.Infra.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class lancamentoOriginal : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LancamentoParcelado", "LancamentoOriginalId", c => c.Guid(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.LancamentoParcelado", "LancamentoOriginalId");
        }
    }
}
