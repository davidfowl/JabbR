namespace JabbR.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBannedIPsTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BannedIPs",
                c => new
                    {
                        Key = c.Int(nullable: false, identity: true),
                        When = c.DateTimeOffset(nullable: false),
                        RemoteIP = c.String(),
                    })
                .PrimaryKey(t => t.Key);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.BannedIPs");
        }
    }
}
