namespace BuyUnion.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProductProxy : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProductProxies",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ProductID = c.Int(nullable: false),
                        ProxyUserID = c.String(),
                        Count = c.Int(nullable: false),
                        Max = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            AddColumn("dbo.Products", "Commission", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "Commission");
            DropTable("dbo.ProductProxies");
        }
    }
}
