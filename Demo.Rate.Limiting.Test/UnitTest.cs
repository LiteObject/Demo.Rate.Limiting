using Demo.Api.Entities;
using Demo.Api.Rules;
using Demo.Api.Storage;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace Demo.Rate.Limiting.Test
{
    public class UnitTest
    {
        private readonly ITestOutputHelper _output;

        public UnitTest(ITestOutputHelper output) => _output = output ?? throw new ArgumentNullException($"{nameof(output)} cannot be null.");


        [Fact]
        public async Task Should_Reject_Five_Req_In_Five_Secs()
        {
            // ARRANGE

            var token = "US123";
            var existingRecord = new Request() { Token = token, Count = 5, First = DateTime.Now.AddSeconds(-1), Last = DateTime.Now };

            var options = new DbContextOptionsBuilder<RequestDbContext>()
            .UseInMemoryDatabase(databaseName: "MyTestDb")
            .Options;

            /*using var context = new RequestDbContext(options);
            await context.Requests.AddAsync(existingRecord);
            await context.SaveChangesAsync(); */

            var contextFactoryMock = new Mock<IDbContextFactory<RequestDbContext>>();
            contextFactoryMock
                .Setup(f => f.CreateDbContextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(() =>
                {
                    var c = new RequestDbContext(options);
                    if (!c.Requests.Any(r => r.Token.Equals(token)))
                    {
                        c.Requests.Add(existingRecord);
                        c.SaveChanges();
                    }
                    return c;
                });

            var rule = new RateLimitingRuleForUS(contextFactoryMock.Object)
            {
                TimeSpanInSec = 5,
                MaxReqCount = 5
            };

            // ACT
            // await rule.AddRequestRecordAsync(existingRecord);
            var reject = await rule.RejectAsync(token);

            _output.WriteLine($"Reject? {reject}");

            // ASSERT
            Assert.True(reject);
        }
    }
}