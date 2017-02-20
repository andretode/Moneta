namespace Moneta.Infra.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class importarOfx : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Lancamento", "NumeroDocumento", c => c.String(maxLength: 100, unicode: false));
            AddColumn("dbo.Lancamento", "Observacao", c => c.String(maxLength: 100, unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Lancamento", "Observacao");
            DropColumn("dbo.Lancamento", "NumeroDocumento");
        }
    }
}
