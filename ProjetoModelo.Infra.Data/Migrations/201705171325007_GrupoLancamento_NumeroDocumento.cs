namespace Moneta.Infra.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GrupoLancamento_NumeroDocumento : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GrupoLancamento", "NumeroDocumento", c => c.String(maxLength: 100, unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.GrupoLancamento", "NumeroDocumento");
        }
    }
}
