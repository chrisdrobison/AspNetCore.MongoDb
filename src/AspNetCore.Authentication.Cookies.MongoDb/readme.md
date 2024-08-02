An ASP.NET Core cookie session store backed by MongoDB.

Example:

```csharp
builder
    .Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.SessionStore = new MongoDbTicketStore(
            new MongoDbTicketStoreOptions { Database = mongoDatabase, CollectionName = "authTicketStore" }
        );
    })
```