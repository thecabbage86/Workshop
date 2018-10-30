﻿
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using System.IO;
using Microsoft.Extensions.Logging;
using System;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using System.Net.Http.Headers;

namespace Fortune_Teller_UI.Services
{
    public class FortuneServiceClient : IFortuneService
    {

        ILogger<FortuneServiceClient> _logger;
        IOptionsSnapshot<FortuneServiceOptions> _config;

        private FortuneServiceOptions Config
        {
            get
            {
                return _config.Value;
            }
        }

        public FortuneServiceClient(
            IOptionsSnapshot<FortuneServiceOptions> config, 
            ILogger<FortuneServiceClient> logger)
        {
            _logger = logger;
            _config = config;
        }

        public async Task<List<Fortune>> AllFortunesAsync()
        {
            return await HandleRequest<List<Fortune>>(Config.AllFortunesURL);
        }

        public async Task<Fortune> RandomFortuneAsync()
        {
            return await HandleRequest<Fortune>(Config.RandomFortuneURL);
        }

        private async Task<T> HandleRequest<T>(string url) where T : class
        {
            _logger?.LogDebug("FortuneService call: {url}", url);
            try
            {
                using (var client = await GetClientAsync())
                {
                    var stream = await client.GetStreamAsync(url);
                    var result = Deserialize<T>(stream);
                    _logger?.LogDebug("FortuneService returned: {result}", result);
                    return result;
                }
            }
            catch (Exception e)
            {
                _logger?.LogError("FortuneService exception: {0}", e);
                throw;
            }
        }


        private T Deserialize<T>(Stream stream) where T : class
        {
            try
            {
                using (JsonReader reader = new JsonTextReader(new StreamReader(stream)))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    return (T)serializer.Deserialize(reader, typeof(T));
                }
            }
            catch (Exception e)
            {
                _logger?.LogError("FortuneService serialization exception: {0}", e);
            }
            return (T)null;
        }

        private async Task<HttpClient> GetClientAsync()
        {
            var client = new HttpClient();
            return await Task.FromResult(client);
        }
    }
}