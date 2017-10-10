namespace MalalimAdmin.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _default : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tbl_AdminRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.tbl_AdminUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.tbl_AdminRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.tbl_AdminUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.tbl_AdminUsers",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.UserId)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.tbl_AdminUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.tbl_AdminUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.tbl_AdminUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.tbl_AdminUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.tbl_AdminUserRoles", "UserId", "dbo.tbl_AdminUsers");
            DropForeignKey("dbo.tbl_AdminUserLogins", "UserId", "dbo.tbl_AdminUsers");
            DropForeignKey("dbo.tbl_AdminUserClaims", "UserId", "dbo.tbl_AdminUsers");
            DropForeignKey("dbo.tbl_AdminUserRoles", "RoleId", "dbo.tbl_AdminRoles");
            DropIndex("dbo.tbl_AdminUserLogins", new[] { "UserId" });
            DropIndex("dbo.tbl_AdminUserClaims", new[] { "UserId" });
            DropIndex("dbo.tbl_AdminUsers", "UserNameIndex");
            DropIndex("dbo.tbl_AdminUserRoles", new[] { "RoleId" });
            DropIndex("dbo.tbl_AdminUserRoles", new[] { "UserId" });
            DropIndex("dbo.tbl_AdminRoles", "RoleNameIndex");
            DropTable("dbo.tbl_AdminUserLogins");
            DropTable("dbo.tbl_AdminUserClaims");
            DropTable("dbo.tbl_AdminUsers");
            DropTable("dbo.tbl_AdminUserRoles");
            DropTable("dbo.tbl_AdminRoles");
        }
    }
}
