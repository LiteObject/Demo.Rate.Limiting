namespace Demo.Api.Attributes
{
    /// <summary>
    /// More on writing "Custom Attributes":
    /// https://docs.microsoft.com/en-us/dotnet/standard/attributes/writing-custom-attributes
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class RateLimiterAttribute : Attribute
    {
        public Type? RateLimitingRule { get; set; }

        public int TimeSpanInSec { get; set; }

        public int MaxReqCount { get; set; }
    }
}
