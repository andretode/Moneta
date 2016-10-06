namespace Moneta.Infra.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class teste : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Conta", "ResultadoValidacao_Mensagem");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Conta", "ResultadoValidacao_Mensagem", c => c.String(maxLength: 100));
        }
    }
}
