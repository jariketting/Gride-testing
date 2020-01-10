using Microsoft.AspNetCore.Mvc.Testing;
using System;
using Xunit;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using AngleSharp.Html.Dom;
using AngleSharp.Dom;
using System.Collections.Generic;
using Newtonsoft.Json;
using Gride.Models;
using System.Net;
using ASIO = AngleSharp.Io;

namespace GrideTest
{
    public class TestAuthHandlerWorks : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public TestAuthHandlerWorks(IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new[] { new Claim(ClaimTypes.Name, "Test user") };
            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "Test");

            var result = AuthenticateResult.Success(ticket);

            return Task.FromResult(result);
        }
    }

	internal static class WorksData
	{
		public static readonly Dictionary<string, HttpContent> postValues = new Dictionary<string, HttpContent>()
		{
			{ "/Works/Delete/1", new StringContent("")},
            { "/Works/Create", new StringContent(JsonConvert.SerializeObject(new
			{
				work = new Work()
				{
					EmployeeID = 1,
					ShiftID = 1,
					Overtime = 0,
					Delay = 0
				}
			}))},
            { "/Works/Details/1", new StringContent("")},
            { "/Works/Edit/1", new StringContent(JsonConvert.SerializeObject(new
			{
				OverTime = 2,
				Delay = 1
			}))}
		};

		public static readonly Dictionary<string, List<(string element, (string name, string value) attribute)>> FormData = new Dictionary<string, List<(string element, (string name, string value) attribute)>>
		{
			{ "/Works/Edit/1", new List<(string element, (string name, string value) attribute)>
			{
				(@".//input[@id=""EmployeeID""]",("value","1")),
				(@".//input[@id=""ShiftID""]",("value","1")),
                (@".//input[@id=""Overtime""]",("value","0")),
                (@".//input[@id=""Delay""]",("value","0"))
            }},
            { "/Works/Details/1", new List<(string element, (string name, string value) attribute)>
            {
                (@".//input[@id=""EmployeeID""]",("value","1")),
                (@".//input[@id=""ShiftID""]",("value","1")),
                (@".//input[@id=""Overtime""]",("value","0")),
                (@".//input[@id=""Delay""]",("value","0"))
            }},
            { "/Works/Create", new List<(string element, (string name, string value) attribute)>
			{
                (@".//input[@id=""EmployeeID""]",("value","1")),
                (@".//input[@id=""ShiftID""]",("value","1")),
                (@".//input[@id=""Overtime""]",("value","0")),
                (@".//input[@id=""Delay""]",("value","0"))
            }},
			{ "/Works/Delete/1", new List<(string element, (string name, string value) attribute)>()}
		};
	}

	public class UnitTestWorks : IClassFixture<WebApplicationFactory<Gride.Startup>>
    {
        private readonly HttpClient Client;
        private readonly WebApplicationFactory<Gride.Startup> _factory;

        public UnitTestWorks(WebApplicationFactory<Gride.Startup> factory)
        {
            _factory = factory;

            var testName = "Test";

            Client = factory.WithWebHostBuilder(builder => builder.ConfigureTestServices(services => services.AddAuthentication(testName).AddScheme<AuthenticationSchemeOptions, TestAuthHandlerWorks>(testName, Options => { })))
                .CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(testName);
        }

        [Theory]
        [InlineData("/Works/Edit/1")]
        [InlineData("/Works/Delete/1")]
        [InlineData("/Works/Details/1")]
        [InlineData("/Works/Create")]
        public async Task WorksTest(string url)
        {
            HttpResponseMessage defPage = await Client.GetAsync(url);
            IHtmlDocument document = await HtmlHelpers.GetDocumentAsync(defPage);

            // makes the page ready to posted
            foreach ((string element, (string name, string value) attribute) data in Data.FormData[url])
            {
                IElement element = document.QuerySelector(data.element);
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

            IHtmlFormElement form = (IHtmlFormElement)document.QuerySelector(".//main//form");
            IHtmlButtonElement submit = (IHtmlButtonElement)document.QuerySelector(@".//input[@type=""submit""]");
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
            Assert.Equal("/Works/", response.Headers.Location.OriginalString);
        }
    }
}
