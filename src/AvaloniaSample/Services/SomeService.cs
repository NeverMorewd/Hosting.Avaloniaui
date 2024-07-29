using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaSample.Services
{
    public class SomeService : ISomeService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        public SomeService(IServiceProvider serviceProvider, 
            IConfiguration configuration,
            ILogger<SomeService> logger) 
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;
            _logger = logger;
        }

        public IEnumerable<KeyValuePair<string, string?>> GetConfigurations()
        {
            _logger?.LogInformation("GetConfigurations");
            return _configuration.AsEnumerable();
        }

        public int GetSomeNumber()
        {
            return new Random().Next();
        }
    }
}
