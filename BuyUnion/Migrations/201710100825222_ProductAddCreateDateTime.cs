namespace BuyUnion.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProductAddCreateDateTime : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "CreateDateTime", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "CreateDateTime");
        }
    }
}
