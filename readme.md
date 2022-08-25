# What is rate limiting?
Rate limiting is the concept of limiting how much a resource can be accessed.

There are multiple different rate limiting algorithms to control the flow of requests. 
We’ll go over 4 of them that will be provided in .NET 7.

## Concurrency limit
- Concurrency limiter limits how many concurrent requests can access a resource.
- If your limit is 10, then 10 requests can access a resource at once and the 11th request will not be allowed.

## Token bucket limit

## Fixed window limit

## Sliding window limit
