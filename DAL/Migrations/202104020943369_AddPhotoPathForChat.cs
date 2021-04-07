namespace DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPhotoPathForChat : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Chats", "PhotoPath", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Chats", "PhotoPath");
        }
    }
}
