namespace JabbR.Models.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddRoomAllowanceForUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("ChatUsers", "RoomAllowance", c => c.Int(nullable: false,defaultValue:5));
            
        }
        
        public override void Down()
        {
            DropColumn("ChatUsers", "RoomAllowance");
        }
    }
}
