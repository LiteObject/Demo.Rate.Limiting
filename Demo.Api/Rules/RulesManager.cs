namespace Demo.Api.Rules
{
    public class RulesManager : IRulesManager
    {
        private readonly List<IRateLimitingRule> rateLimitingRules = new();

        public void SetRateLimitingRule(IRateLimitingRule rule)
        {
            rateLimitingRules.Add(rule);
        }

        public void ClearRateLimitingRules()
        {
            rateLimitingRules.Clear();
        }

        public async Task<bool> RejectAsync(string token)
        {
            var result = false;

            if (string.IsNullOrEmpty(token) || token.Length < 2)
            {
                return result;
            }

            var tokenPrefix = token.Substring(0, 2);
            var eligibleRulesForToken = rateLimitingRules.Where(r => r.GetType().Name.Contains(tokenPrefix)).ToList();

            foreach (var rule in eligibleRulesForToken)
            {
                result = await rule.RejectAsync(token);

                if (result)
                {
                    break;
                }
            }

            return result;
        }
    }
}
