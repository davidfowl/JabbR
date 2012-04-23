namespace JabbR.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class Test : DbMigration
    {
        public override void Up()
        {
            DropTable("EdmMetadata");
        }
        
        public override void Down()
        {
            CreateTable(
                "EdmMetadata",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ModelHash = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
    }
}
