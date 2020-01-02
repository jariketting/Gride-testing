using System.Net.Http;
using System;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

//Deze class bevat de client die ik gebruik in mijn Messagecontorller class
namespace IntegrationTests
{

    public class _IntegrationTest
    {
        public readonly HttpClient _client;
        public _IntegrationTest()
        {
            var appFactory = new WebApplicationFactory<Gride.Startup>();
            _client = appFactory.CreateClient();
        }

    }
}
