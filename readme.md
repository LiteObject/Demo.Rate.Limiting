# What is rate limiting?
Rate limiting is the concept of limiting how much a resource can be accessed.

## About this project:
- This is a custom implementation (without worrying too much about algo)
- Rate limiting can be applied to action method using one or more attributes as shown below:
```csharp
[RateLimiter(RateLimitingRule = typeof(RateLimitingRuleForUS), MaxReqCount = 5, TimeSpanInSec = 30)]
[RateLimiter(RateLimitingRule = typeof(RateLimitingRuleForEU))]
[HttpGet(Name = "GetWeatherForecast")]
public IEnumerable<WeatherForecast> Get()
{
}
```

- Either max request count within a time range can be specifid from the attribue or it can be configued centrally in the `RateLimitingMiddleware.cs` file.
- In this example, rate limiting rule(s) is picked based on header (token) value. This loggic is within `RulesManager.cs` file. 
 However, this matching of header value with rule(s) can be easily removed.



