using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Relational.Migrations.Infrastructure;
using UnicornStore.AspNet.Models.Identity;

namespace UnicornStore.Migrations
{
    [ContextType(typeof(ApplicationDbContext))]
    partial class CreateIdentitySchema
    {
        public override string Id
        {
            get { return "20150723210554_CreateIdentitySchema"; }
        }
        
        public override string ProductVersion
        {
            get { return "7.0.0-beta5-13549"; }
        }
        
        public override void BuildTargetModel(ModelBuilder builder)
        {
            builder
                .Annotation("SqlServer:ValueGeneration", "Identity");
            
            builder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .GenerateValueOnAdd();
                    
                    b.Property<string>("ConcurrencyStamp")
                        .ConcurrencyToken();
                    
                    b.Property<string>("Name");
                    
                    b.Property<string>("NormalizedName");
                    
                    b.Key("Id");
                    
                    b.Annotation("Relational:TableName", "AspNetRoles");
                });
            
            builder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .GenerateValueOnAdd()
                        .StoreGeneratedPattern(StoreGeneratedPattern.Identity);
                    
                    b.Property<string>("ClaimType");
                    
                    b.Property<string>("ClaimValue");
                    
                    b.Property<string>("RoleId");
                    
                    b.Key("Id");
                    
                    b.Annotation("Relational:TableName", "AspNetRoleClaims");
                });
            
            builder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .GenerateValueOnAdd()
                        .StoreGeneratedPattern(StoreGeneratedPattern.Identity);
                    
                    b.Property<string>("ClaimType");
                    
                    b.Property<string>("ClaimValue");
                    
                    b.Property<string>("UserId");
                    
                    b.Key("Id");
                    
                    b.Annotation("Relational:TableName", "AspNetUserClaims");
                });
            
            builder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .GenerateValueOnAdd();
                    
                    b.Property<string>("ProviderKey")
                        .GenerateValueOnAdd();
                    
                    b.Property<string>("ProviderDisplayName");
                    
                    b.Property<string>("UserId");
                    
                    b.Key("LoginProvider", "ProviderKey");
                    
                    b.Annotation("Relational:TableName", "AspNetUserLogins");
                });
            
            builder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");
                    
                    b.Property<string>("RoleId");
                    
                    b.Key("UserId", "RoleId");
                    
                    b.Annotation("Relational:TableName", "AspNetUserRoles");
                });
            
            builder.Entity("UnicornStore.AspNet.Models.Identity.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .GenerateValueOnAdd();
                    
                    b.Property<int>("AccessFailedCount");
                    
                    b.Property<string>("ConcurrencyStamp")
                        .ConcurrencyToken();
                    
                    b.Property<string>("Email");
                    
                    b.Property<bool>("EmailConfirmed");
                    
                    b.Property<bool>("LockoutEnabled");
                    
                    b.Property<DateTimeOffset?>("LockoutEnd");
                    
                    b.Property<string>("NormalizedEmail");
                    
                    b.Property<string>("NormalizedUserName");
                    
                    b.Property<string>("PasswordHash");
                    
                    b.Property<string>("PhoneNumber");
                    
                    b.Property<bool>("PhoneNumberConfirmed");
                    
                    b.Property<string>("SecurityStamp");
                    
                    b.Property<bool>("TwoFactorEnabled");
                    
                    b.Property<string>("UserName");
                    
                    b.Key("Id");
                    
                    b.Annotation("Relational:TableName", "AspNetUsers");
                });
            
            builder.Entity("UnicornStore.AspNet.Models.Identity.PreApproval", b =>
                {
                    b.Property<string>("UserEmail")
                        .GenerateValueOnAdd();
                    
                    b.Property<string>("Role")
                        .GenerateValueOnAdd();
                    
                    b.Property<string>("ApprovedBy");
                    
                    b.Property<DateTime>("ApprovedOn");
                    
                    b.Key("UserEmail", "Role");
                    
                    b.Annotation("Relational:TableName", "AspNetPreApprovals");
                });
            
            builder.Entity("UnicornStore.AspNet.Models.Identity.UserAddress", b =>
                {
                    b.Property<int>("UserAddressId")
                        .GenerateValueOnAdd()
                        .StoreGeneratedPattern(StoreGeneratedPattern.Identity);
                    
                    b.Property<string>("Addressee")
                        .Required();
                    
                    b.Property<string>("CityOrTown")
                        .Required();
                    
                    b.Property<string>("Country")
                        .Required();
                    
                    b.Property<string>("LineOne")
                        .Required();
                    
                    b.Property<string>("LineTwo");
                    
                    b.Property<string>("StateOrProvince")
                        .Required();
                    
                    b.Property<string>("UserId")
                        .Required();
                    
                    b.Property<string>("ZipOrPostalCode")
                        .Required();
                    
                    b.Key("UserAddressId");
                    
                    b.Annotation("Relational:TableName", "AspNetUserAddresses");
                });
            
            builder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityRoleClaim<string>", b =>
                {
                    b.Reference("Microsoft.AspNet.Identity.EntityFramework.IdentityRole")
                        .InverseCollection()
                        .ForeignKey("RoleId");
                });
            
            builder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserClaim<string>", b =>
                {
                    b.Reference("UnicornStore.AspNet.Models.Identity.ApplicationUser")
                        .InverseCollection()
                        .ForeignKey("UserId");
                });
            
            builder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserLogin<string>", b =>
                {
                    b.Reference("UnicornStore.AspNet.Models.Identity.ApplicationUser")
                        .InverseCollection()
                        .ForeignKey("UserId");
                });
            
            builder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserRole<string>", b =>
                {
                    b.Reference("Microsoft.AspNet.Identity.EntityFramework.IdentityRole")
                        .InverseCollection()
                        .ForeignKey("RoleId");
                    
                    b.Reference("UnicornStore.AspNet.Models.Identity.ApplicationUser")
                        .InverseCollection()
                        .ForeignKey("UserId");
                });
            
            builder.Entity("UnicornStore.AspNet.Models.Identity.UserAddress", b =>
                {
                    b.Reference("UnicornStore.AspNet.Models.Identity.ApplicationUser")
                        .InverseCollection()
                        .ForeignKey("UserId");
                });
        }
    }
}
