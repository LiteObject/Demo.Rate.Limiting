namespace Demo.Api.Rules
{
    public interface IRateLimitingRule
    {
        public Task<bool> RejectAsync(string token);
    }
}
