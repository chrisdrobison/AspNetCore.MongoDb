using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using MongoDB.Driver;

namespace AspNetCore.Authentication.Cookies.MongoDb
{
    public class MongoDbTicketStore : ITicketStore
    {
        private readonly MongoDbTicketStoreOptions _options;
        private readonly IMongoCollection<Ticket> _ticketCollection;

        public MongoDbTicketStore(MongoDbTicketStoreOptions options)
        {
            options.ValidateOptions();
            _options = options;
            _ticketCollection = options.Database.GetCollection<Ticket>(options.CollectionName);
        }

        public async Task<string> StoreAsync(AuthenticationTicket ticket)
        {
            var userId = ticket.Principal.Identity?.Name;
            if (!string.IsNullOrEmpty(_options.NameClaimType))
            {
                userId = ticket.Principal.FindFirst(_options.NameClaimType)?.Value;
            }

            var dbTicket = new Ticket()
            {
                Expires = ticket.Properties.ExpiresUtc?.UtcDateTime,
                LastActivity = DateTime.UtcNow,
                Value = TicketSerializer.Default.Serialize(ticket),
                UserId = userId
            };
            await _ticketCollection.InsertOneAsync(dbTicket);
            return dbTicket.Id.ToString();
        }

        public async Task RenewAsync(string key, AuthenticationTicket ticket)
        {
            if (!Guid.TryParse(key, out var id))
            {
                return;
            }

            var dbTicket = await _ticketCollection.Find(t => t.Id == id).FirstOrDefaultAsync();
            if (dbTicket == null)
            {
                return;
            }

            dbTicket.Value = TicketSerializer.Default.Serialize(ticket);
            dbTicket.LastActivity = DateTime.UtcNow;
            dbTicket.Expires = ticket.Properties.ExpiresUtc?.UtcDateTime;
            await _ticketCollection.ReplaceOneAsync(t => t.Id == id, dbTicket);
        }

        public async Task<AuthenticationTicket> RetrieveAsync(string key)
        {
            if (!Guid.TryParse(key, out var id))
            {
                return null;
            }

            var dbTicket = await _ticketCollection.Find(t => t.Id == id).FirstOrDefaultAsync();
            if (dbTicket == null)
            {
                return null;
            }

            await _ticketCollection.UpdateOneAsync(t => t.Id == id,
                Builders<Ticket>.Update.Set(t => t.LastActivity, DateTime.UtcNow));

            return TicketSerializer.Default.Deserialize(dbTicket.Value);
        }

        public async Task RemoveAsync(string key)
        {
            if (!Guid.TryParse(key, out var id))
            {
                return;
            }

            await _ticketCollection.DeleteOneAsync(t => t.Id == id);
        }
    }
}
