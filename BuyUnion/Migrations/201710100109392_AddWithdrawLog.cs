namespace BuyUnion.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddWithdrawLog : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.WithdrawLogs",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        UserID = c.String(),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                        State = c.Int(nullable: false),
                        PayType = c.String(),
                        PayNumber = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.WithdrawLogs");
        }
    }
}
