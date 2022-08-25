using Demo.Api.Attributes;
using Demo.Api.Rules;
using Demo.Api.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using System.Net;

namespace Demo.Api.Middlewares
{
    public class RateLimitingMiddleware
    {
        private const string TokenHeaderKey = "X-Token";
        private readonly IRulesManager _rulesManager;
        private readonly RequestDelegate _next;

        public RateLimitingMiddleware(RequestDelegate next, IRulesManager rulesManager)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _rulesManager = rulesManager ?? throw new ArgumentNullException(nameof(rulesManager));
        }

        public async Task Invoke(HttpContext context, IDbContextFactory<RequestDbContext> contextFactory)
        {
            var endpoint = context.GetEndpoint();

            // var attribute = endpoint?.Metadata.GetMetadata<RateLimiterAttribute>();
            var attributes = endpoint?.Metadata.GetOrderedMetadata<RateLimiterAttribute>();

            if (attributes == null || attributes.Count == 0)
            {
                await _next(context);
                return;
            }

            _rulesManager.ClearRateLimitingRules();

            foreach (var attr in attributes)
            {
                var timeSpanInSec = attr.GetType().GetProperty(nameof(RateLimiterAttribute.TimeSpanInSec))?.GetValue(attr, null);
                var maxReqCount = attr.GetType().GetProperty(nameof(RateLimiterAttribute.MaxReqCount))?.GetValue(attr, null);

                var ruleType = attr.GetType().GetProperty(nameof(RateLimiterAttribute.RateLimitingRule))?.GetValue(attr, null);
                var ruleTypeName = ruleType?.ToString();

                if (!string.IsNullOrWhiteSpace(ruleTypeName))
                {
                    var type = Type.GetType(ruleTypeName);

                    if (type is null)
                    {
                        continue;
                    }

                    var ruleInstance = Activator.CreateInstance(type, contextFactory) as RateLimitingRuleBase;

                    if (ruleInstance is not null)
                    {
                        ruleInstance.TimeSpanInSec = Convert.ToInt32(timeSpanInSec);
                        ruleInstance.MaxReqCount = Convert.ToInt32(maxReqCount);
                        _rulesManager.SetRateLimitingRule(ruleInstance);
                    }
                }
            }

            var token = GetToken(context);

            if (await _rulesManager.RejectAsync(token))
            {
                context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                return;
            }

            await _next(context);
        }

        private string? GetToken(HttpContext context)
        {
            context.Request.Headers.TryGetValue(TokenHeaderKey, out StringValues tokens);
            return tokens.FirstOrDefault();
        }
    }
}
