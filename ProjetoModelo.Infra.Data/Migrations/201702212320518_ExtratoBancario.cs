namespace Moneta.Infra.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ExtratoBancario : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ExtratoBancario",
                c => new
                    {
                        ExtratoBancarioId = c.Guid(nullable: false),
                        Descricao = c.String(nullable: false, maxLength: 100, unicode: false),
                        Valor = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DataCompensacao = c.DateTime(nullable: false, precision: 0),
                        NumeroDocumento = c.String(nullable: false, maxLength: 100, unicode: false),
                        ContaId = c.Guid(nullable: false),
                        DataCadastro = c.DateTime(nullable: false, precision: 0),
                    })
                .PrimaryKey(t => t.ExtratoBancarioId)
                .ForeignKey("dbo.Conta", t => t.ContaId)
                .Index(t => t.ContaId);
            
            DropColumn("dbo.Lancamento", "NumeroDocumento");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Lancamento", "NumeroDocumento", c => c.String(maxLength: 100, unicode: false));
            DropForeignKey("dbo.ExtratoBancario", "ContaId", "dbo.Conta");
            DropIndex("dbo.ExtratoBancario", new[] { "ContaId" });
            DropTable("dbo.ExtratoBancario");
        }
    }
}
