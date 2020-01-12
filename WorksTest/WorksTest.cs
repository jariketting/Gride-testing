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
	/// <summary>
	/// Storing test names
	/// </summary>
	internal static class Names
	{
		public static readonly string TestName = "Test";
		public static readonly string AdminTestName = "ATest";
		public static readonly string TestAdminName = "0967844@hr.nl";
		public static readonly string TestEmployeeName = "0123456@hr.nl";
	}

	/// <summary>
	/// Form data for testing
	/// </summary>
	internal static class Data
	{
		// store data in dictionary and access by page url.
		public static readonly Dictionary<string, List<(string element, (string name, string value) attribute)>> FormData = new Dictionary<string, List<(string element, (string name, string value) attribute)>>
		{
			// Edit page
			{ "/Works/Edit/1", new List<(string element, (string name, string value) attribute)>
			{
				(@".//select[@id=""EmployeeID""]/option[@value=""2""]", ("selected","selected")),
				(@".//select[@id=""ShiftID""]/option[@value=""2""]", ("selected","selected")),
				(@".//input[@id=""Overtime""]",("value","1")),
				(@".//input[@id=""Delay""]",("value","1"))
			}},
			// Create page
			{ "/Works/Create", new List<(string element, (string name, string value) attribute)>
			{
				(@".//select[@id=""EmployeeID""]/option[@value=""1""]", ("selected","selected")),
				(@".//select[@id=""ShiftID""]/option[@value=""1""]", ("selected","selected")),
				(@".//input[@id=""Overtime""]",("value","0")),
				(@".//input[@id=""Delay""]",("value","0"))
			}},
			// Delete has no data for us to set
			{ "/Works/Delete/1", new List<(string element, (string name, string value) attribute)>()}
		};
	}

	/// <summary>
	/// Test class for works
	/// </summary>
	public class WorksTest : IClassFixture<WebApplicationFactory<Startup>>
	{
		private readonly WebApplicationFactory<Startup> Factory;

		/// <summary>
		/// Init WorksTests and set factory
		/// </summary>
		/// <param name="factory"></param>
		public WorksTest(WebApplicationFactory<Startup> factory)
		{
			Factory = factory;
		}

		/// <summary>
		/// Tests index, edit, create, delete and details
		/// </summary>
		/// <param name="url"></param>
		/// <returns>Result</returns>
		[Theory]
		[InlineData("/Works/")]
		[InlineData("/Works/Index")]
		[InlineData("/Works/Edit/1")]
		[InlineData("/Works/Delete/1")]
		[InlineData("/Works/Details/1")]
		[InlineData("/Works/Create")]
		public async Task WorksControllerTest(string url)
		{
			// Create client
			HttpClient client = Factory.CreateClient();

			// get url (as seen in InlineData)
			HttpResponseMessage response = await client.GetAsync(url);

			// Make sure response code is 200 (OK)
			response.EnsureSuccessStatusCode();

			// check if response is correct (type must be text/html and in utf-8 charset)
			Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType.ToString());
		}
	}
	/// <summary>
	/// Test user not logged in
	/// </summary>
	public class NotLogedInTests :
		IClassFixture<WorksTestFactory<Startup>>
	{
		private readonly HttpClient Client;
		private readonly WorksTestFactory<Startup> Factory;

		/// <summary>
		/// Set factory and create client
		/// </summary>
		/// <param name="factory"></param>
		public NotLogedInTests(WorksTestFactory<Startup> factory)
		{
			Factory = factory;
			Client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
		}

		/// <summary>
		/// Test index, edit, delete, details and create page
		/// </summary>
		/// <param name="url"></param>
		/// <returns>Result</returns>
		[Theory]
		[InlineData("/Works/")]
		[InlineData("/Works/Index")]
		[InlineData("/Works/Edit/1")]
		[InlineData("/Works/Delete/1")]
		[InlineData("/Works/Details/1")]
		[InlineData("/Works/Create")]
		public async Task WorksAccesWhileNotLoggedIn(string url)
		{
			// get page by url (as in InlineData)
			HttpResponseMessage Page = await Client.GetAsync(url);

			// make sure user gets redirected to login page
			Assert.Equal(HttpStatusCode.Redirect, Page.StatusCode);
			Assert.StartsWith("http://localhost/Identity/Account/Login", Page.Headers.Location.OriginalString);
		}
	}

	/// <summary>
	/// Tests logged in users
	/// </summary>
	public class LoggedInTests :
		IClassFixture<WorksTestFactory<Startup>>
	{
		private readonly HttpClient Client;
		private readonly WorksTestFactory<Startup> Factory;

		/// <summary>
		/// Build factory
		/// </summary>
		/// <param name="factory"></param>
		public LoggedInTests(WorksTestFactory<Startup> factory)
		{
			Factory = factory;
			Client = factory.WithWebHostBuilder(builder => builder.ConfigureTestServices(services => services.AddAuthentication(Names.TestName).AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(Names.TestName, Options => { })))
				.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
			Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Names.TestName);
		}

		/// <summary>
		/// Test index, edit, delete, details and create page
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		[Theory]
		[InlineData("/Works/")]
		[InlineData("/Works/Index")]
		[InlineData("/Works/Edit/1")]
		[InlineData("/Works/Delete/1")]
		[InlineData("/Works/Details/1")]
		[InlineData("/Works/Create")]
		public async Task WorksAccesToUnAllowedPlaces(string url)
		{
			// get page by url proved in InlineData
			HttpResponseMessage Page = await Client.GetAsync(url);

			// User should not have access to this page
			Assert.Equal(HttpStatusCode.Forbidden, Page.StatusCode);
		}
	}

	/// <summary>
	/// Tests for logged in admin
	/// </summary>
	public class AdminLogedInTests :
		IClassFixture<WorksTestFactory<Startup>>
	{
		private readonly WorksTestFactory<Startup> Factory;
		private readonly HttpClient Client;

		/// <summary>
		/// Build factory
		/// </summary>
		/// <param name="factory"></param>
		public AdminLogedInTests(WorksTestFactory<Startup> factory)
		{
			Factory = factory;
			Client = Factory.WithWebHostBuilder(builder => builder.ConfigureTestServices(services => services.AddAuthentication(Names.AdminTestName).AddScheme<AuthenticationSchemeOptions, AdminTestAuthHandler>(Names.AdminTestName, Options => { })))
				.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
			Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Names.AdminTestName);
		}

		/// <summary>
		/// Test admin access to pages
		/// Tests index, edit, delete, details and create
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		[Theory]
		[InlineData("/Works/")]
		[InlineData("/Works/Index")]
		[InlineData("/Works/Edit/1")]
		[InlineData("/Works/Delete/1")]
		[InlineData("/Works/Details/1")]
		[InlineData("/Works/Create")]
		public async Task WorksAccesToAllowedPlaces(string url)
		{
			// Get page by url from InlineData
			HttpResponseMessage Page = await Client.GetAsync(url);

			// User should have access to page
			Assert.Equal(HttpStatusCode.OK, Page.StatusCode);
		}

		/// <summary>
		/// Test pages without data
		/// Tests edit, delete and create
		/// </summary>
		/// <param name="url"></param>
		/// <returns>Result</returns>
		[Theory]
		[InlineData("/Works/Edit/1")]
		[InlineData("/Works/Delete/1")]
		[InlineData("/Works/Create")]
		public async Task NoDataTest(string url)
		{
			// get pages by url and give no data
			HttpResponseMessage Page = await Client.PostAsync(url, null);

			// server should respond with bad request
			Assert.Equal(HttpStatusCode.BadRequest, Page.StatusCode);
		}

		/// <summary>
		/// Angle sharp tests for Edit, Delete and Create
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		[Theory]
		[InlineData("/Works/Edit/1")]
		[InlineData("/Works/Delete/1")]
		[InlineData("/Works/Create")]
		public async Task AngleSharpTests(string url)
		{
			// Get page by url
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

			// Submit form
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

	/// <summary>
	/// Init works test
	/// </summary>
	/// <typeparam name="TSartup">The startup class type used by this factory</typeparam>
	public class WorksTestFactory<TSartup> :
		WebApplicationFactory<TSartup> where TSartup : class
	{
		protected override void ConfigureWebHost(IWebHostBuilder builder)
		{
			// Configure 
			builder.ConfigureServices(services =>
			{
				// Add database into memory
				IServiceProvider serviceProvider = new ServiceCollection().
				AddEntityFrameworkInMemoryDatabase().BuildServiceProvider();

				// add context to databse and call it TestDataBase
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
						// Some debug stuff
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

	/// <summary>
	/// Handler for testing authentication
	/// </summary>
	public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
	{
		/// <summary>
		/// Init
		/// </summary>
		/// <param name="options"></param>
		/// <param name="logger"></param>
		/// <param name="encoder"></param>
		/// <param name="clock"></param>
		public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
		ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) :
			base(options, logger, encoder, clock)
		{
		}

		/// <summary>
		/// Async task handler
		/// </summary>
		/// <returns>Result</returns>
		protected override Task<AuthenticateResult> HandleAuthenticateAsync()
		{
			Claim[] claims = new[] { new Claim(ClaimTypes.Name, Names.TestEmployeeName) }; // build claim with test employee
			ClaimsIdentity identity = new ClaimsIdentity(claims, Names.TestName); // add identity
			ClaimsPrincipal principal = new ClaimsPrincipal(identity);
			AuthenticationTicket ticket = new AuthenticationTicket(principal, Names.TestName);

			AuthenticateResult res = AuthenticateResult.Success(ticket);

			return Task.FromResult(res);
		}
	}

	/// <summary>
	/// Test handler for admin user
	/// </summary>
	public class AdminTestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
	{
		/// <summary>
		/// Init Admin test handler
		/// </summary>
		/// <param name="options"></param>
		/// <param name="logger"></param>
		/// <param name="encoder"></param>
		/// <param name="clock"></param>
		public AdminTestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
		ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) :
			base(options, logger, encoder, clock)
		{
		}

		/// <summary>
		/// Handle authenticated task
		/// </summary>
		/// <returns>Result</returns>
		protected override Task<AuthenticateResult> HandleAuthenticateAsync()
		{
			Claim[] claims = new[] { new Claim(ClaimTypes.Name, Names.TestAdminName) }; // create claim with test admin name
			ClaimsIdentity identity = new ClaimsIdentity(claims, Names.AdminTestName); // create identity with test admin name
			ClaimsPrincipal principal = new ClaimsPrincipal(identity);
			AuthenticationTicket ticket = new AuthenticationTicket(principal, Names.AdminTestName);

			AuthenticateResult res = AuthenticateResult.Success(ticket);

			return Task.FromResult(res);
		}
	}
}