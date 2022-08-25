using Demo.Api.Entities;
using Demo.Api.Storage;
using Microsoft.EntityFrameworkCore;

namespace Demo.Api.Rules
{
    public class RateLimitingRuleForUS : RateLimitingRuleBase
    {
        public RateLimitingRuleForUS(IDbContextFactory<RequestDbContext> contextFactory) : base(contextFactory)
        {
            Console.WriteLine($">>> Instantiating \"{nameof(RateLimitingRuleForUS)}\" class.");
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

            var elapsed = DateTime.Now - request.First;

            if (elapsed.TotalSeconds <= TimeSpanInSec && request.Count >= MaxReqCount)
            {
                Console.WriteLine($">>> REJECTED. Current Time: {DateTime.Now}, First Req Time: {request.First}, Elapsed {elapsed.TotalSeconds} s., Req Count: {request.Count}");
                return true;
            }

            await UpdateRequestRecord(request);

            return false;
        }
    }
}
