
using Fortune_Teller_Service.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fortune_Teller_Service.Controllers
{
    [Route("api/[controller]")]
    public class FortunesController : Controller
    {
        ILogger<FortunesController> _logger;
        IFortuneRepository _fortuneRepository;


        public FortunesController(ILogger<FortunesController> logger, IFortuneRepository fortuneRepository)
        {
            _logger = logger;
            _fortuneRepository = fortuneRepository;
        }


        // GET: api/fortunes/all
        [HttpGet("all")]
        public async Task<List<Fortune>> AllFortunesAsync()
        {
            _logger?.LogDebug("AllFortunesAsync");

            var fortunes = await _fortuneRepository.GetAllAsync();

            var fortuneDtos = new List<Fortune>();
            foreach(var fortune in fortunes)
            {
                fortuneDtos.Add(new Fortune() { Id = fortune.Id, Text = fortune.Text });
            }
            return fortuneDtos;
        }

        // GET api/fortunes/random
        [HttpGet("random")]
        public async Task<Fortune> RandomFortuneAsync()
        {
            _logger?.LogDebug("RandomFortuneAsync");

            var fortune = await _fortuneRepository.RandomFortuneAsync();
            return new Fortune() { Id = fortune.Id, Text = fortune.Text };
        }
    }
}
