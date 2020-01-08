using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Gride;
using System.Threading.Tasks;
using Assert = Xunit.Assert;
using System.Net;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Collections.Generic;
using AngleSharp.Html.Dom;
using Xunit;

using ASIO = AngleSharp.Io;
using AngleSharp.Dom;
using AngleSharp.XPath;
using System;

namespace GrideTest.Helpers
{
    internal static class Data
    {
        public static readonly Dictionary<string, List<(string element, (string name, string value) attribute)>> FormData = new Dictionary<string, List<(string element, (string name, string value) attribute)>>
        {
            { "/Skills/Edit/1", new List<(string element, (string name, string value) attribute)>
            {
				(@".//input[@id=""Name""]",("value","Eating"))
            }},
            { "/Skills/Create", new List<(string element, (string name, string value) attribute)>
            {
                (@"//*[@id=""Name""]",("value","Dancing"))
            }},
			// delete has no data for us to set
			{ "/Skills/Delete/1", new List<(string element, (string name, string value) attribute)>()},
        
        };
    }
    public class SkillsLocationsTest : IClassFixture<WebApplicationFactory<Gride.Startup>>
    {
        private readonly WebApplicationFactory<Gride.Startup> _factory;

        public SkillsLocationsTest(WebApplicationFactory<Gride.Startup> factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("/Skills")]
        [InlineData("/Skills/index")]
        [InlineData("/Skills/delete/2")]
        [InlineData("/Skills/create")]
        [InlineData("/SKills/edit/2")]
        [InlineData("/Location")]
        [InlineData("/Location/index")]
        [InlineData("/Location/create")]
        [InlineData("/Location/delete/2")]
        [InlineData("/Location/edit/2")]
        public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        [Theory]
        [InlineData("/Skills")]
        [InlineData("/Skills/index")]
        [InlineData("/Skills/delete/2")]
        [InlineData("/Skills/edit/2")]
        [InlineData("/Skills/create")]
        [InlineData("/Location")]
        [InlineData("/Location/index")]
        [InlineData("/Location/create")]
        [InlineData("/Location/edit/2")]
        [InlineData("/Location/delete/2")]
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
        [InlineData("/Skills")]
        [InlineData("/Skills/index")]
        [InlineData("/Skills/delete/2")]
        [InlineData("/Skills/create")]
        [InlineData("/SKills/edit/2")]
        [InlineData("/Location")]
        [InlineData("/Location/index")]
        [InlineData("")]
        [InlineData("/Location/delete/2")]
        [InlineData("/Location/edit/2")]
        public async Task Get_SecurePageIsReturnedForAnAuthenticatedUser(string url)
        {
            // Arrange
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddAuthentication("Test")
                        .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                            "Test", options => { });
                });
            })
                .CreateClient(new WebApplicationFactoryClientOptions
                {
                    AllowAutoRedirect = false,
                });

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Test");

            //Act
            var response = await client.GetAsync(url);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        internal static class Names
        {
            public static readonly string TestName = "Test";
            public static readonly string AdminTestName = "ATest";
            public static readonly string TestAdminName = "0967844@hr.nl";
            public static readonly string TestEmployeeName = "0123456@hr.nl";
        }

        [Theory]
        [InlineData("/Skills")]
        [InlineData("/Skills/index")]
        [InlineData("/Skills/delete/2")]
        [InlineData("/Skills/create")]
        [InlineData("/SKills/edit/2")]
        [InlineData("/Location")]
        [InlineData("/Location/index")]
        [InlineData("/Location/delete/2")]
        [InlineData("/Location/edit/2")]
        public async Task Get_PageWhenAuthenticated(string url)
        {
            //Arrange
            var client = _factory.WithWebHostBuilder(builder => builder.ConfigureTestServices(services => services.AddAuthentication(Names.TestName).AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(Names.TestName, Options => { })))
                .CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Names.TestName);

            //Act
            HttpResponseMessage Page = await client.GetAsync(url);
            //Assert
            Assert.Equal(HttpStatusCode.Forbidden, Page.StatusCode);
        }

    }

    public class AdminLogedInTests :
    IClassFixture<ShiftTestFactory<Startup>>
    {
        private readonly ShiftTestFactory<Startup> Factory;
        private readonly HttpClient Client;

        public AdminLogedInTests(ShiftTestFactory<Startup> factory)
        {
            Factory = factory;
            Client = Factory.WithWebHostBuilder(builder => builder.ConfigureTestServices(services => services.AddAuthentication(Names.AdminTestName).AddScheme<AuthenticationSchemeOptions, AdminTestAuthHandler>(Names.AdminTestName, Options => { })))
                .CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Names.AdminTestName);
        }

        [Theory]
        [InlineData("/Skills/Edit/1")]
        [InlineData("/Skills/Delete/1")]
        [InlineData("/Skills/Create")]
        public async Task AngleSharpTests(string url)
        {
            HttpResponseMessage defPage = await Client.GetAsync(url);
            IHtmlDocument document = await HtmlHelpers.GetDocumentAsync(defPage);
            // makes the page ready to posted
            foreach ((string element, (string name, string value) attribute) data in Data.FormData[url])
            {
                IElement element = (IElement)document.Body.SelectSingleNode(data.element);
                if (data.attribute.value == null)
                {
                    if (element.HasAttribute(data.attribute.name))
                        element.RemoveAttribute(data.attribute.name);
                }
                else
                {
                    element.SetAttribute(data.attribute.name, data.attribute.value);
                }
            }
            IHtmlFormElement form = (IHtmlFormElement)document.Body.SelectSingleNode("./html/body/div/main/div[1]/div/form");
            IHtmlInputElement submit = (IHtmlInputElement)document.Body.SelectSingleNode(@".//input[@type=""submit""]");
            ASIO.DocumentRequest request = form.GetSubmission(submit);
            Uri target = request.Target;
            if (submit.HasAttribute("formaction"))
                target = new Uri(submit.GetAttribute("formaction"), UriKind.Relative);
            HttpRequestMessage message = new HttpRequestMessage(new HttpMethod(request.Method.ToString()), target) { Content = new StreamContent(request.Body) };

            foreach (KeyValuePair<string, string> header in request.Headers)
            {
                message.Headers.TryAddWithoutValidation(header.Key, header.Value);
                message.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            HttpResponseMessage response = await Client.SendAsync(message);
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        }
    }

}
