namespace BuyUnion.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProductAddState : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "State", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "State");
        }
    }
}
