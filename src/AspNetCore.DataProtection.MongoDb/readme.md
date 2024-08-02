An ASP.NET Core Data Protection key repository that uses MongoDB.

Example usage:

```csharp
builder.Services.AddDataProtection().PersistKeysToMongoDb(() => mongoDatabase);
```