namespace Demo.Api.Rules
{
    public interface IRulesManager
    {
        public void SetRateLimitingRule(IRateLimitingRule rule);
        public void ClearRateLimitingRules();
        public Task<bool> RejectAsync(string token);
    }
}
