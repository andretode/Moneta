namespace Moneta.Infra.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class categoria : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Categoria",
                c => new
                    {
                        CategoriaId = c.Guid(nullable: false),
                        Descricao = c.String(nullable: false, maxLength: 100),
                        Cor = c.String(nullable: false, maxLength: 100),
                        DataCadastro = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.CategoriaId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Categoria");
        }
    }
}
