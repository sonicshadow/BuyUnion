namespace BuyUnion.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddProduct : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ShopID = c.Int(nullable: false),
                        Name = c.String(),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OriginalPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Stock = c.Int(nullable: false),
                        Sales = c.Int(nullable: false),
                        Image = c.String(),
                        DetailsImage = c.String(),
                        Remark = c.String(),
                        ExpressTemplateID = c.Int(),
                        Weight = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Products");
        }
    }
}
