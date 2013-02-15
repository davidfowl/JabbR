namespace JabbR.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RoomIsConnectionMessageDisplayed : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ChatRooms", "IsConnectionMessageDisplayed", c => c.Boolean(nullable: false, defaultValue: true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ChatRooms", "IsConnectionMessageDisplayed");
        }
    }
}
