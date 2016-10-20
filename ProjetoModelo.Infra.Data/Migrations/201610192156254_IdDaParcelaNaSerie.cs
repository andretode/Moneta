namespace Moneta.Infra.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IdDaParcelaNaSerie : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Lancamento", "IdDaParcelaNaSerie", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Lancamento", "IdDaParcelaNaSerie");
        }
    }
}
