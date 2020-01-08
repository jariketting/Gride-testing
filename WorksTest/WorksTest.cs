using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.XPath;

using Gride;
using Gride.Data;
using Gride.Models;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

using Xunit;

using ASIO = AngleSharp.Io;

namespace GrideTest.WorksTest
{
	internal static class Names
	{
		public static readonly string TestName = "Test";
		public static readonly string AdminTestName = "ATest";
		public static readonly string TestAdminName = "0967844@hr.nl";
		public static readonly string TestEmployeeName = "0123456@hr.nl";
	}

	internal static class Data
	{
		public static readonly Dictionary<string, List<(string element, (string name, string value) attribute)>> FormData = new Dictionary<string, List<(string element, (string name, string value) attribute)>>
		{
			{ "/Works/Edit/1", new List<(string element, (string name, string value) attribute)>
			{
				(@".//select[@id=""EmployeeID""]/option[@value=""2""]", ("selected","selected")),
				(@".//select[@id=""ShiftID""]/option[@value=""2""]", ("selected","selected")),
				(@".//input[@id=""Overtime""]",("value","1")),
				(@".//input[@id=""Delay""]",("value","1"))
			}},
			{ "/Works/Create", new List<(string element, (string name, string value) attribute)>
			{
				(@".//select[@id=""EmployeeID""]/option[@value=""1""]", ("selected","selected")),
				(@".//select[@id=""ShiftID""]/option[@value=""1""]", ("selected","selected")),
				(@".//input[@id=""Overtime""]",("value","0")),
				(@".//input[@id=""Delay""]",("value","0"))
			}},
			// delete has no data for us to set
			{ "/Works/Delete/1", new List<(string element, (string name, string value) attribute)>()}
		};
	}

	public class WorksTest : IClassFixture<WebApplicationFactory<Startup>>
	{
		private readonly WebApplicationFactory<Startup> Factory;

		public WorksTest(WebApplicationFactory<Startup> factory)
		{
			Factory = factory;
		}

		[Theory]
		[InlineData("/Works/")]
		[InlineData("/Works/Index")]
		[InlineData("/Works/Edit/1")]
		[InlineData("/Works/Delete/1")]
		[InlineData("/Works/Details/1")]
		[InlineData("/Works/Create")]
		public async Task WorksControllerTest(string url)
		{
			HttpClient client = Factory.CreateClient();

			HttpResponseMessage response = await client.GetAsync(url);

			response.EnsureSuccessStatusCode();
			Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType.ToString());
		}
	}

	public class NotLogedInTests :
		IClassFixture<WorksTestFactory<Startup>>
	{
		private readonly HttpClient Client;
		private readonly WorksTestFactory<Startup> Factory;

		public NotLogedInTests(WorksTestFactory<Startup> factory)
		{
			Factory = factory;
			Client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
		}

		[Theory]
		[InlineData("/Works/")]
		[InlineData("/Works/Index")]
		[InlineData("/Works/Edit/1")]
		[InlineData("/Works/Delete/1")]
		[InlineData("/Works/Details/1")]
		[InlineData("/Works/Create")]
		public async Task WorksAccesWhileNotLoggedIn(string url)
		{
			HttpResponseMessage Page = await Client.GetAsync(url);
			Assert.Equal(HttpStatusCode.Redirect, Page.StatusCode);
			Assert.StartsWith("http://localhost/Identity/Account/Login", Page.Headers.Location.OriginalString);
		}
	}

	public class LogedInTests :
		IClassFixture<WorksTestFactory<Startup>>
	{
		private readonly HttpClient Client;
		private readonly WorksTestFactory<Startup> Factory;

		public LogedInTests(WorksTestFactory<Startup> factory)
		{
			Factory = factory;
			Client = factory.WithWebHostBuilder(builder => builder.ConfigureTestServices(services => services.AddAuthentication(Names.TestName).AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(Names.TestName, Options => { })))
				.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
			Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Names.TestName);
		}

		[Theory]
		[InlineData("/Works/")]
		[InlineData("/Works/Index")]
		[InlineData("/Works/Edit/1")]
		[InlineData("/Works/Delete/1")]
		[InlineData("/Works/Details/1")]
		[InlineData("/Works/Create")]
		public async Task WorksAccesToUnAllowedPlaces(string url)
		{
			HttpResponseMessage Page = await Client.GetAsync(url);
			Assert.Equal(HttpStatusCode.Forbidden, Page.StatusCode);
		}
	}

	public class AdminLogedInTests :
		IClassFixture<WorksTestFactory<Startup>>
	{
		private readonly WorksTestFactory<Startup> Factory;
		private readonly HttpClient Client;

		public AdminLogedInTests(WorksTestFactory<Startup> factory)
		{
			Factory = factory;
			Client = Factory.WithWebHostBuilder(builder => builder.ConfigureTestServices(services => services.AddAuthentication(Names.AdminTestName).AddScheme<AuthenticationSchemeOptions, AdminTestAuthHandler>(Names.AdminTestName, Options => { })))
				.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
			Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Names.AdminTestName);
		}

		[Theory]
		[InlineData("/Works/")]
		[InlineData("/Works/Index")]
		[InlineData("/Works/Edit/1")]
		[InlineData("/Works/Delete/1")]
		[InlineData("/Works/Details/1")]
		[InlineData("/Works/Create")]
		public async Task WorksAccesToAllowedPlaces(string url)
		{
			HttpResponseMessage Page = await Client.GetAsync(url);
			Assert.Equal(HttpStatusCode.OK, Page.StatusCode);
		}

		[Theory]
		[InlineData("/Works/Edit/1")]
		[InlineData("/Works/Delete/1")]
		[InlineData("/Works/Create")]
		public async Task NoDataTest(string url)
		{
			HttpResponseMessage Page = await Client.PostAsync(url, null);
			Assert.Equal(HttpStatusCode.BadRequest, Page.StatusCode);
		}

		[Theory]
		[InlineData("/Works/Edit/1")]
		[InlineData("/Works/Delete/1")]
		[InlineData("/Works/Create")]
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
			IHtmlFormElement form = (IHtmlFormElement)document.Body.SelectSingleNode(".//main//form");
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

	public class WorksTestFactory<TSartup> :
		WebApplicationFactory<TSartup> where TSartup : class
	{
		protected override void ConfigureWebHost(IWebHostBuilder builder)
		{
			builder.ConfigureServices(services =>
			{
				IServiceProvider serviceProvider = new ServiceCollection().
				AddEntityFrameworkInMemoryDatabase().BuildServiceProvider();

				services.AddDbContext<ApplicationDbContext>(op => { op.UseInMemoryDatabase("TestDataBase"); op.UseInternalServiceProvider(serviceProvider); });

				ServiceProvider sp = services.BuildServiceProvider();

				using (IServiceScope scope = sp.CreateScope())
				{
					IServiceProvider ss = scope.ServiceProvider;
					ApplicationDbContext db = ss.GetRequiredService<ApplicationDbContext>();
					ILogger<WorksTestFactory<TSartup>> logger = ss.GetRequiredService<ILogger<WorksTestFactory<TSartup>>>();
					db.Database.EnsureCreated();

					try
					{
						Utils.InitDB(db);
						Debug.WriteLine("[000] First Email: " + db.EmployeeModel.ToList()[0].EMail + " is Admin: " + db.EmployeeModel.ToList()[0].Admin); ;
						Debug.WriteLine("[001] Is Admin Name Admin: " + db.EmployeeModel.First(e => e.EMail == Names.TestAdminName).Admin);
					}
					catch (Exception ex)
					{
						logger.LogError(ex, "Error in the initialization of the database\n{0}", ex.Message);
					}
				}
			});
		}
	}

	public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
	{
		public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
		ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) :
			base(options, logger, encoder, clock)
		{
		}

		protected override Task<AuthenticateResult> HandleAuthenticateAsync()
		{
			Claim[] claims = new[] { new Claim(ClaimTypes.Name, Names.TestEmployeeName) };
			ClaimsIdentity identity = new ClaimsIdentity(claims, Names.TestName);
			ClaimsPrincipal principal = new ClaimsPrincipal(identity);
			AuthenticationTicket ticket = new AuthenticationTicket(principal, Names.TestName);

			AuthenticateResult res = AuthenticateResult.Success(ticket);

			return Task.FromResult(res);
		}
	}

	public class AdminTestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
	{
		public AdminTestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
		ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) :
			base(options, logger, encoder, clock)
		{
		}

		protected override Task<AuthenticateResult> HandleAuthenticateAsync()
		{
			Claim[] claims = new[] { new Claim(ClaimTypes.Name, Names.TestAdminName) };
			ClaimsIdentity identity = new ClaimsIdentity(claims, Names.AdminTestName);
			ClaimsPrincipal principal = new ClaimsPrincipal(identity);
			AuthenticationTicket ticket = new AuthenticationTicket(principal, Names.AdminTestName);

			AuthenticateResult res = AuthenticateResult.Success(ticket);

			return Task.FromResult(res);
		}
	}
}