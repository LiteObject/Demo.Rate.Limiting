using Demo.Api.Entities;
using Demo.Api.Storage;
using Microsoft.EntityFrameworkCore;

namespace Demo.Api.Rules
{
    internal class RateLimitingRuleForEU : RateLimitingRuleBase
    {
        public RateLimitingRuleForEU(IDbContextFactory<RequestDbContext> contextFactory) : base(contextFactory)
        {
            Console.WriteLine($">>> Instantiating \"{nameof(RateLimitingRuleForEU)}\" class.");
        }

        public override async Task<bool> RejectAsync(string token)
        {
            var request = await FindRequestRecordAsync(token);

            if (request is null)
            {
                var newReq = new Request { Token = token, Count = 1, First = DateTime.Now, Last = DateTime.Now };
                await AddRequestRecordAsync(newReq);
                return false;
            }

            // Rule: Reject if more than 1 call within 5 sec period.
            var elapsedSinceLastCall = DateTime.Now - request.Last;

            if (elapsedSinceLastCall.TotalSeconds < TimeSpanInSec && request.Count > 0)
            {
                Console.WriteLine($">>> REJECTED. Current Time: {DateTime.Now}, Last Req Time: {request.Last}, Elapsed {elapsedSinceLastCall.TotalSeconds} s., Req Count: {request.Count}");
                return true;
            }

            await UpdateRequestRecord(request);

            return false;
        }
    }
}
