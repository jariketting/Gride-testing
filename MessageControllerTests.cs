using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Gride;
using Xunit;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using GrideTest;
using Microsoft.AspNetCore.Authentication;
using System.Net.Http.Headers;
using System.Net.Http;

using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.XPath;

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
        [Theory]
        [InlineData("/Message")]
        [InlineData("/Message/Create")]
        public async Task Get_PageWhenAuthenticated(string url)
        {
            //Arrange
            var client = _factory.WithWebHostBuilder(builder => builder.ConfigureTestServices(services => services.AddAuthentication(Names.TestName).AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(Names.TestName, Options => { })))
                .CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Names.TestName);

            //Act
            HttpResponseMessage Page = await client.GetAsync(url);
            //Assert
            Assert.Equal(HttpStatusCode.OK, Page.StatusCode);
        }
       
        [Theory]
        [InlineData("/Message/Create")]
        public async Task Warning_WhenNoText(string url)
        {
            //Arrange
            HttpResponseMessage page = await _client.GetAsync(url);
            IHtmlDocument document = await HtmlHelpers.GetDocumentAsync(page);
            IHtmlFormElement form = (IHtmlFormElement)document.Body.SelectSingleNode(@".//form[@id=""messageForm""]");
            IHtmlInputElement textField = (IHtmlInputElement)document.Body.SelectSingleNode(@".//input[@id=""textInput""]");
            IHtmlInputElement submitButton = (IHtmlInputElement)document.Body.SelectSingleNode(@".//input[@id=""submitForNewMessage""]");

            //Act


            if (form != null)
            {
                textField.Value = "aaaaaaaaaaaaaaaaaaaaaaaaa";
                await form.SubmitAsync(submitButton);
            }
            //Arrange

            Assert.Equal(HttpStatusCode.OK, page.StatusCode);
        }
    }
}
