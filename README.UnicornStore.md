# Unicorn Store

Rowan Miller presented his **UnicornStore** sample at **Ignite 2015**. Watch his session [here](https://channel9.msdn.com/Events/Ignite/2015/BRK3727) to learn about the present state of Entity Framework 7 (EF7) and the range of platforms and technologies in which it can play a productive part.

[Rowan's source repository on github](https://github.com/rowanmiller/UnicornStore) is split into a "StartingSource" and "CompletedSource" folders. Each of these folders contains four projects.

This document describes the **UnicornStore MVC Web App** project within the "CompletedSource" folder.

## Prerequisites

### ASP.NET 5 (Beta 4)

Install as described [here](http://docs.asp.net/en/latest/).

Make sure that you have the release nuget feed for the Beta 4 release among your package sources (it may not be). In Visual Studio, 

- from the menu: "Tools | Nuget Package Manager | Package Manager Settings"
- select Package Sources
- look for a source targeting `https://www.myget.org/F/aspnetrelease/api/v2`
- if it's not there ... add it.

### SQL Server
The connection string (in *config.json*) points to a localDb SQL Server named `(localdb)\MSSQLLocalDB` and a database named "UnicornStore". These instructions assume you're running on Windows and mostly using Visual Studio 2015 RC which means we can assume that "localDb" is installed.

## Demo Installation

* Add a `c:/temp` directory (`c:/temp/DatabaseLog.sql`) ... unless you already have this directory. It will hold the SqlLogger output file, *DatabaseLog.sql*.


* Clone the Unicorn demo repository: `git clone https://github.com/rowanmiller/UnicornStore.git`

* Add a *secrets.json* file to the UnicoreStore project directory. **(to be explained)**

## Try it

You'll be working with the ASP.NET web app in the "CompletedSource" folder. 

You can run it from VS2015 or from the command line 

>Don't try both simultaneously; there will be a port conflict.
 
### Visual Studio 2015 RC

* Change the working directory: `cd ~\UnicornStore\CompletedSource\UnicornStore`
* Open the "UnicornStore.sln" solution file in Visual Studio 2015 RC
* Restore packages
	* select the **UnicornStore** project
	* open the context menu (right mouse click)
	* run "Restore Packages"
	* confirm that bower and npm were run; *wwwroot/lib* should hold client packages such as "bootstrap"
* Build it
* F5 or (Ctrl-F5)

The browser should open automatically at `http://localhost:5000/`

>If the app launches but the home page is ugly, you missed the package restore or the bower and npm commands weren't run. Try "Restore Packages" again or restore the packages through the command line (see next).

### Command line

>You must close the VS solution first to run from the command line or you'll get a port conflict.

* Change the working directory: `cd ~\UnicornStore\CompletedSource\UnicornStore\src\UnicornStore`
* Restore packages: `dnu restore`
* Launch the server with IISExpress: `dnx . web`
* Open a browser to `http://localhost:5000/`


## Highlights

There is cool EF7 stuff in here.

### EF Logging

Review

* *Logging/__SqlLogger.cs__*
* *Logging/__SqlLoggerProvider.cs__*

To see the output after running the app,

* Open a command window
* `notepad c:/temp/DatabaseLog.sql`

Notice that a simple query output actually looks simple! No more wrapping every query in a wacky `select` statement.

### Chained *Include*

EF7 makes it easier to include related entities down "dotted paths". For example, while getting the "Order" headers, you could want their "Line" items and also each item's "Product".

The `Include` expression for that was kind of nasty in earlier EF versions. EF7 adds a new `ThenInclude` API that's more natural as seen in the `Index` method of the `OrdersController` (*Controllers/OrdersController.cs*)

	var orders = db.Orders
	    .Include(o => o.Lines).ThenInclude(l => l.Product)

The generated SQL has changed to. In earlier EF versions, this query would resolve into a single SQL statement with two left order joins. Each row of the result would be a LineItem w/ both Order and Product data, most of it duplicated across the result set.

The EF7 SQL provider breaks a query that includes a collection ("Lines" in this case) into two separate queries (look at the last two queries in the log):

1. A query returning Orders
2. A query returning a projection of the LineItems and their Products

The size of the combined query result data is much smaller now (even if the Product data are duplicated).

Of course EF hides this behavior from the developer. It quietly constructs the complete in-memory graph out of these separate query results.

This "query splitting" is a behavior of the SQL Provider. Other database providers makes their own decisions about how to process queries. Now they have the option of returning multiple result sets.

### Batch Insert/Update

EF7 can send multiple SQL insert and update commands to the database in a single batch. To see this in action:

-  Go to the Bulk Price Reduction feature (`Admin | Bulk Price Reduction`).
-  Pick a populated category like "Mens Clothing"
-  Set a percent reduction
-  Click "Save"

The current price for all items of "Mens Clothing" will be reduced from MSRP by that percentage.

This action is implemented by the `BulkPriceReduction` POST method in the `ManageProductsController` controller (*Controllers/ManageProductsController.cs*) which retrieves the selected products, calculates the new price for each product, and saves the changes.

In earlier EF versions, if there were four changed products, there would be four separate database update requests.

In EF7 there is just one request with four updates ... as the SQL log confirms:

	SET NOCOUNT OFF;
	UPDATE [Product] SET [CurrentPrice] = @p0 WHERE [ProductId] = @p1;
	SELECT @@ROWCOUNT;
	UPDATE [Product] SET [CurrentPrice] = @p2 WHERE [ProductId] = @p3;
	SELECT @@ROWCOUNT;
	UPDATE [Product] SET [CurrentPrice] = @p4 WHERE [ProductId] = @p5;
	SELECT @@ROWCOUNT;
	UPDATE [Product] SET [CurrentPrice] = @p6 WHERE [ProductId] = @p7;
	SELECT @@ROWCOUNT;

### Composable SQL Expressions

The database has a `SearchProducts` **table function** that performs a text search on the "DisplayName" and "Description" fields of the "Products" table and returns a projection.

EF can't map this table function to an entity. We'll need to write some SQL. 

You could issue raw SQL commands through EF *before* EF7. Now you can also **compose** those commands with LINQ expressions.

See the `Search` method in the `ShopController` (*Controllers/__ShopController.cs__*)

	var products = db.Products
	    .FromSql("SELECT * FROM [dbo].[SearchProducts] (@p0)", term)
	    .OrderByDescending(p => p.Savings)
	    .ToList();

The `.OrderBy..` is composed on the SQL "SELECT ...".

### Query on CLR calculated properties

The "SearchProducts" query returns a Product projection sorted by the matching items with the biggest savings.

**`Savings` isn't a column in the database! It's a calculated read-only property** in the `Product` class (*Models/UnicornStore/Product.cs*):

	public decimal Savings
	{
	    get { return MSRP - CurrentPrice; }
	}

This query would fail before EF7 because everything in a query had to resolve to a column in a table in the database. This property isn't in the database. It only exists on the CLR class. 

EF7 can split the query work to run partially on the database and partially on the client ... as seen here.

>Use this feature with great care! An ill-advised query could trigger a massive download of data for post-processing in memory. Filters with calculated fields are particularly dangerous.

### Shadow State

In EF7 you can define a "Shadow State" property that is part of the Entity model but not part of the corresponding CLR class.

An example of that in the UnicornStore is the `LastUpdated` model property which maps to the "LastUpdated" column of the ***CartItem*** table. For some reason, we don't want to expose a `CartItem.LastUpdated` property to the application.

We can't just ignore it. We are responsible for updating that column in the CartItem database table when we insert or update a CartItem.

In old EF, we'd *have* to add this property to the `CartItem` class. We don't have to do that in EF7. Instead we can add it to the CartItem ***mapping*** as a "Shadow State" property. Here's how:

* Open the `UnicornStoreContext` (*Models/UnicornStore/UnicornStoreContext.cs*).
* Scroll to `OnModelCreating` where we define mappings with the fluent API
* Find the configuration for `CartItem`

		builder.Entity<CartItem>().Property<DateTime>("LastUpdated");

This overload of the `Property` configuration method takes the data type and the name of the Shadow Property rather than a standard mapping expression referencing a property of the CLR class. It adds the property to the "CartItem" model even though it's not in the `CartItem` class.

Now we have to use it ... that is, we have to set it whenever we create or update a `CartItem` instance. 

* Return to the `UnicornStoreContext`
* Scroll to the override of `SaveChanges` which
	* calls `DetectChanges` first
	* finds added/updated `CartItem` entities
	* sets the `LastUpdated` Shadow State property ***by name***

			entry.Property("LastUpdated").CurrentValue = DateTime.UtcNow;

	* invokes the base `SaveChanges`

After adding an item to the shopping cart, look at the SQL log. You'll see that the "LastUpdated" column value is both retrieved and set. EF simply ignores that value when materializing `CartItem` instances in the application.

>It is not yet possible to compose a query that mentions "LastUpdated"; that feature is coming.

## UnicornStore Database

After you've run the app once (which creates the db with EF migrations), you can explore the db from the Server Explorer window in Visual Studio. The SQL Server Object Explorer window in VS may help you find the `(localdb)\MSSQLLocalDB` server.