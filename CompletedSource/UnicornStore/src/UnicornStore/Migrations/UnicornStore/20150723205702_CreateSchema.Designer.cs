using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Relational.Migrations.Infrastructure;
using UnicornStore.AspNet.Models.UnicornStore;

namespace UnicornStore.Migrations
{
    [ContextType(typeof(UnicornStoreContext))]
    partial class CreateSchema
    {
        public override string Id
        {
            get { return "20150723205702_CreateSchema"; }
        }
        
        public override string ProductVersion
        {
            get { return "7.0.0-beta5-13549"; }
        }
        
        public override void BuildTargetModel(ModelBuilder builder)
        {
            builder
                .Annotation("SqlServer:ValueGeneration", "Identity");
            
            builder.Entity("UnicornStore.AspNet.Models.UnicornStore.CartItem", b =>
                {
                    b.Property<int>("CartItemId")
                        .GenerateValueOnAdd()
                        .StoreGeneratedPattern(StoreGeneratedPattern.Identity);
                    
                    b.Property<DateTime>("LastUpdated");
                    
                    b.Property<DateTime>("PriceCalculated");
                    
                    b.Property<decimal>("PricePerUnit");
                    
                    b.Property<int>("ProductId");
                    
                    b.Property<int>("Quantity");
                    
                    b.Property<string>("Username");
                    
                    b.Key("CartItemId");
                });
            
            builder.Entity("UnicornStore.AspNet.Models.UnicornStore.Category", b =>
                {
                    b.Property<int>("CategoryId")
                        .GenerateValueOnAdd()
                        .StoreGeneratedPattern(StoreGeneratedPattern.Identity);
                    
                    b.Property<string>("DisplayName");
                    
                    b.Property<int?>("ParentCategoryId");
                    
                    b.Key("CategoryId");
                });
            
            builder.Entity("UnicornStore.AspNet.Models.UnicornStore.Order", b =>
                {
                    b.Property<int>("OrderId")
                        .GenerateValueOnAdd()
                        .StoreGeneratedPattern(StoreGeneratedPattern.Identity);
                    
                    b.Property<DateTime>("CheckoutBegan");
                    
                    b.Property<DateTime?>("OrderPlaced");
                    
                    b.Property<int>("State");
                    
                    b.Property<decimal>("Total");
                    
                    b.Property<string>("Username");
                    
                    b.Key("OrderId");
                });
            
            builder.Entity("UnicornStore.AspNet.Models.UnicornStore.OrderLine", b =>
                {
                    b.Property<int>("OrderId");
                    
                    b.Property<int>("ProductId");
                    
                    b.Property<decimal>("PricePerUnit");
                    
                    b.Property<int>("Quantity");
                    
                    b.Key("OrderId", "ProductId");
                });
            
            builder.Entity("UnicornStore.AspNet.Models.UnicornStore.OrderShippingDetails", b =>
                {
                    b.Property<int>("OrderId");
                    
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
                    
                    b.Property<string>("ZipOrPostalCode")
                        .Required();
                    
                    b.Key("OrderId");
                });
            
            builder.Entity("UnicornStore.AspNet.Models.UnicornStore.Product", b =>
                {
                    b.Property<int>("ProductId")
                        .GenerateValueOnAdd()
                        .StoreGeneratedPattern(StoreGeneratedPattern.Identity);
                    
                    b.Property<int>("CategoryId");
                    
                    b.Property<decimal>("CurrentPrice");
                    
                    b.Property<string>("Description");
                    
                    b.Property<string>("DisplayName");
                    
                    b.Property<string>("ImageUrl");
                    
                    b.Property<decimal>("MSRP");
                    
                    b.Key("ProductId");
                });
            
            builder.Entity("UnicornStore.AspNet.Models.UnicornStore.WebsiteAd", b =>
                {
                    b.Property<int>("WebsiteAdId")
                        .GenerateValueOnAdd()
                        .StoreGeneratedPattern(StoreGeneratedPattern.Identity);
                    
                    b.Property<string>("Details");
                    
                    b.Property<DateTime?>("End");
                    
                    b.Property<string>("ImageUrl");
                    
                    b.Property<DateTime?>("Start");
                    
                    b.Property<string>("TagLine");
                    
                    b.Property<string>("Url");
                    
                    b.Key("WebsiteAdId");
                });
            
            builder.Entity("UnicornStore.AspNet.Models.UnicornStore.CartItem", b =>
                {
                    b.Reference("UnicornStore.AspNet.Models.UnicornStore.Product")
                        .InverseCollection()
                        .ForeignKey("ProductId");
                });
            
            builder.Entity("UnicornStore.AspNet.Models.UnicornStore.Category", b =>
                {
                    b.Reference("UnicornStore.AspNet.Models.UnicornStore.Category")
                        .InverseCollection()
                        .ForeignKey("ParentCategoryId");
                });
            
            builder.Entity("UnicornStore.AspNet.Models.UnicornStore.OrderLine", b =>
                {
                    b.Reference("UnicornStore.AspNet.Models.UnicornStore.Order")
                        .InverseCollection()
                        .ForeignKey("OrderId");
                    
                    b.Reference("UnicornStore.AspNet.Models.UnicornStore.Product")
                        .InverseCollection()
                        .ForeignKey("ProductId");
                });
            
            builder.Entity("UnicornStore.AspNet.Models.UnicornStore.OrderShippingDetails", b =>
                {
                    b.Reference("UnicornStore.AspNet.Models.UnicornStore.Order")
                        .InverseReference()
                        .ForeignKey("UnicornStore.AspNet.Models.UnicornStore.OrderShippingDetails", "OrderId");
                });
            
            builder.Entity("UnicornStore.AspNet.Models.UnicornStore.Product", b =>
                {
                    b.Reference("UnicornStore.AspNet.Models.UnicornStore.Category")
                        .InverseCollection()
                        .ForeignKey("CategoryId");
                });
        }
    }
}
