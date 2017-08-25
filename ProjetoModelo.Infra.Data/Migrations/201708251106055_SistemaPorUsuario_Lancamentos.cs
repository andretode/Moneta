namespace Moneta.Infra.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SistemaPorUsuario_Lancamentos : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Lancamento", "AppUserId", c => c.Guid(nullable: false));
            CreateIndex("dbo.Lancamento", "AppUserId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Lancamento", new[] { "AppUserId" });
            DropColumn("dbo.Lancamento", "AppUserId");
        }
    }
}
