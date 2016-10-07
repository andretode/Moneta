namespace Moneta.Infra.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class pago : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Lancamento", "Pago", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Lancamento", "Pago");
        }
    }
}
