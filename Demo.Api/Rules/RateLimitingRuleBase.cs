using Demo.Api.Entities;
using Demo.Api.Storage;
using Microsoft.EntityFrameworkCore;

namespace Demo.Api.Rules
{
    public abstract class RateLimitingRuleBase : IRateLimitingRule
    {
        private readonly IDbContextFactory<RequestDbContext> _contextFactory;

        protected RateLimitingRuleBase(IDbContextFactory<RequestDbContext> contextFactory)
        {
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        }

        public int TimeSpanInSec { get; set; }
        public int MaxReqCount { get; set; }

        public async Task<Request?> FindRequestRecordAsync(string token)
        {
            // DbContext instances created in this way are not managed by the application's
            // service provider and therefore must be disposed by the application.
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Requests.FindAsync(token);
        }

        public async Task AddRequestRecordAsync(Request request)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            await context.Requests.AddAsync(request);
            await context.SaveChangesAsync();
        }

        public async Task UpdateRequestRecord(Request request)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            request.Count += 1;
            request.Last = DateTime.Now;

            context.Attach(request);
            // context.Entry(request).State = EntityState.Modified;
            context.Entry(request).Property(r => r.Count).IsModified = true;
            context.Entry(request).Property(r => r.Last).IsModified = true;

            await context.SaveChangesAsync();
        }

        public abstract Task<bool> RejectAsync(string token);
    }
}
