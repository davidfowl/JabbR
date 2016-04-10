namespace JabbR.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRoomPinning : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ChatRooms", "Pinned", c => c.Boolean(nullable: false));
            AddColumn("dbo.ChatRooms", "PinnedPriority", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ChatRooms", "PinnedPriority");
            DropColumn("dbo.ChatRooms", "Pinned");
        }
    }
}
