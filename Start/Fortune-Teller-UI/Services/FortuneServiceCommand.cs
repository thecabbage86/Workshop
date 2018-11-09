using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Steeltoe.CircuitBreaker.Hystrix;

namespace Fortune_Teller_UI.Services
{
    public class FortuneServiceCommand : HystrixCommand<Fortune>
    {
        private readonly IFortuneService _fortuneService;
        private readonly ILogger<FortuneServiceCommand> _logger;

        public FortuneServiceCommand(IHystrixCommandOptions options, IFortuneService fortuneService, ILogger<FortuneServiceCommand> logger) : base(options)
        {
            _fortuneService = fortuneService;
            _logger = logger;
            IsFallbackUserDefined = true;
        }

        public async Task<Fortune> RandomFortuneAsync()
        {
            return await ExecuteAsync();
        }

        protected override async Task<Fortune> RunAsync()
        {
            var fortune = await _fortuneService.RandomFortuneAsync();

            _logger.LogInformation("Run: {0}", fortune);

            return fortune;
        }

        protected override async Task<Fortune> RunFallbackAsync()
        {
            _logger.LogInformation("RunFallback");
            return await Task.FromResult(new Fortune() { Id = 444444, Text = "Your fortune has run out." });
        }
    }
}
