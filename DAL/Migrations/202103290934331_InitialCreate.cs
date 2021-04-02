namespace DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Accounts",
                c => new
                    {
                        ClientId = c.Int(nullable: false, identity: true),
                        Email = c.String(nullable: false, maxLength: 50),
                        Phone = c.String(nullable: false, maxLength: 50),
                        Password = c.String(nullable: false, maxLength: 200),
                    })
                .PrimaryKey(t => t.ClientId);
            
            CreateTable(
                "dbo.Clients",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 100),
                        UniqueName = c.String(),
                        PhotoPath = c.String(),
                        AccountId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Accounts", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.ChatMembers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DateLastReadMessage = c.DateTime(nullable: false),
                        IsAdmin = c.Boolean(nullable: false),
                        ChatId = c.Int(nullable: false),
                        ClientId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Chats", t => t.ChatId)
                .ForeignKey("dbo.Clients", t => t.ClientId)
                .Index(t => t.ChatId)
                .Index(t => t.ClientId);
            
            CreateTable(
                "dbo.Chats",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        UniqueName = c.String(),
                        IsPrivate = c.Boolean(nullable: false),
                        MaxUsers = c.Int(nullable: false),
                        IsPM = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Messages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Text = c.String(),
                        SendingTime = c.DateTime(nullable: false),
                        ClientId = c.Int(nullable: false),
                        ChatId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Chats", t => t.ChatId)
                .ForeignKey("dbo.Clients", t => t.ClientId)
                .Index(t => t.ClientId)
                .Index(t => t.ChatId);
            
            CreateTable(
                "dbo.Files",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FilePath = c.String(nullable: false, maxLength: 50),
                        MessageId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Messages", t => t.MessageId)
                .Index(t => t.MessageId);
            
            CreateTable(
                "dbo.Contacts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClientId = c.Int(nullable: false),
                        ContactClientId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Clients", t => t.ClientId)
                .ForeignKey("dbo.Clients", t => t.ContactClientId)
                .Index(t => t.ClientId)
                .Index(t => t.ContactClientId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Clients", "Id", "dbo.Accounts");
            DropForeignKey("dbo.Contacts", "ContactClientId", "dbo.Clients");
            DropForeignKey("dbo.Contacts", "ClientId", "dbo.Clients");
            DropForeignKey("dbo.ChatMembers", "ClientId", "dbo.Clients");
            DropForeignKey("dbo.ChatMembers", "ChatId", "dbo.Chats");
            DropForeignKey("dbo.Files", "MessageId", "dbo.Messages");
            DropForeignKey("dbo.Messages", "ClientId", "dbo.Clients");
            DropForeignKey("dbo.Messages", "ChatId", "dbo.Chats");
            DropIndex("dbo.Contacts", new[] { "ContactClientId" });
            DropIndex("dbo.Contacts", new[] { "ClientId" });
            DropIndex("dbo.Files", new[] { "MessageId" });
            DropIndex("dbo.Messages", new[] { "ChatId" });
            DropIndex("dbo.Messages", new[] { "ClientId" });
            DropIndex("dbo.ChatMembers", new[] { "ClientId" });
            DropIndex("dbo.ChatMembers", new[] { "ChatId" });
            DropIndex("dbo.Clients", new[] { "Id" });
            DropTable("dbo.Contacts");
            DropTable("dbo.Files");
            DropTable("dbo.Messages");
            DropTable("dbo.Chats");
            DropTable("dbo.ChatMembers");
            DropTable("dbo.Clients");
            DropTable("dbo.Accounts");
        }
    }
}
