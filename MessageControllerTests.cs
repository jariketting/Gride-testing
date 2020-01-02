using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Gride;
using Xunit;

namespace IntegrationTests
{
    public class MessageControllerTests : _IntegrationTest
    {
        private readonly WebApplicationFactory<Startup> _factory = new WebApplicationFactory<Startup>();
        [Theory]
        [InlineData("/")]
        [InlineData("/Message")]
        [InlineData("/Message/Create")]
        public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
        {
            // Arrange
            
            // Act
            var response = await _client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }



        [Theory]
        [InlineData("/Message")]
        [InlineData("/Message/Create")]
        public async Task Get_SecurePageRedirectsAnUnauthenticatedUser(string url)
        {
            // Arrange
            var client = _factory.CreateClient(
                new WebApplicationFactoryClientOptions
                {
                    AllowAutoRedirect = false
                });


            // Act
            var response = await client.GetAsync(url);

            // Assert
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.StartsWith("http://localhost/Identity/Account/Login",
                response.Headers.Location.OriginalString);
        }


    }
}
