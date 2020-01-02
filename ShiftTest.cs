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

namespace GrideTest
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
		public static readonly Dictionary<string, HttpContent> postValues = new Dictionary<string, HttpContent>()
		{
			{ "/Shifts/Delete/1", new StringContent("")},
			{ "/Shifts/DeleteAllFollowing/1", new StringContent("")},
			{ "/Shifts/Create", new StringContent(JsonConvert.SerializeObject(new
			{
				shift = new Shift()
				{
					Start = DateTime.Now,
					End = DateTime.Now.AddHours(2),
					LocationID = 1,
				},
				selectedSkills = new string[]{"1"},
				selectedFunctions = new string[]{"1"},
				selectedFunctionsMax = new int[]{ 3 },
			}))},
			{ "/Shifts/Edit/1", new StringContent(JsonConvert.SerializeObject(new
			{
				selectedSkills = new string[]{"1"},
				selectedFunctions = new string[]{"1"},
				selectedFunctionsMax = new int[]{ 3 },
				selectedEmployees = new string[]{"1"},
			}))},
			{ "/Shifts/AssignEmployee/1", new StringContent(JsonConvert.SerializeObject(new
			{
				EmployeeID = 1,
				FunctionID = 1
			}))}
		};

		public static readonly Dictionary<string, List<(string element, (string name, string value) attribute)>> FormData = new Dictionary<string, List<(string element, (string name, string value) attribute)>>
		{
			{ "/Shifts/Edit/1", new List<(string element, (string name, string value) attribute)>
			{
				// I fucking love xpath
				(@".//input[@id=""Start""]",("value","2019-12-31T00:00:00.000")),
				(@".//input[@id=""End""]",("value","2019-12-31T02:00:00.000")),
				(@".//input[@name=""selectedSkills"" and @value=""1""]", ("checked","checked")),
				(@".//input[@name=""selectedSkills"" and @value=""2""]", ("checked",null)),
				(@".//input[@name=""selectedFunctions"" and @value=""3""]",("checked", "checked")),
				(@".//input[@name=""selectedFunctionsMax"" and ../../td/input[@name=""selectedFunctions"" and @value=""3""]]", ("value", "5"))
			}},
			{ "/Shifts/Create", new List<(string element, (string name, string value) attribute)>
			{
				(@".//input[@id=""Start""]",("value","2019-12-31T00:00:00.000")),
				(@".//input[@id=""End""]",("value","2019-12-31T02:00:00.000")),
				(@".//select[@id=""LocationID""]/option[@value=""1""]", ("selected","selected")),
				(@".//input[@name=""selectedSkills"" and @value=""1""]", ("checked","checked")),
				(@".//input[@name=""selectedSkills"" and @value=""2""]", ("checked",null)),
				(@".//input[@name=""selectedFunctions"" and @value=""3""]",("checked", "checked")),
				(@".//input[@name=""selectedFunctionsMax"" and ../../td/input[@name=""selectedFunctions"" and @value=""3""]]", ("value", "5"))
			}},
			// delete has no data for us to set
			{ "/Shifts/Delete/1", new List<(string element, (string name, string value) attribute)>()},
			{ "/Shifts/DeleteAllFollowing/1", new List<(string element, (string name, string value) attribute)>()},
			{ "/Shifts/DeleteAllFollowing/2", new List<(string element, (string name, string value) attribute)>()},
			{ "/Shifts/AssignEmployee/1", new List<(string element, (string name, string value) attribute)>
			{
				(@".//select[@name=""EmployeeID""]/option[@value=""5""]",("selected","selected")),
				(@".//select[@name=""FunctionID""]/option[1]", ("selected","selected")),
			}}
		};
	}

	public class ShiftTest : IClassFixture<WebApplicationFactory<Startup>>
	{
		private readonly WebApplicationFactory<Startup> Factory;

		public ShiftTest(WebApplicationFactory<Startup> factory)
		{
			Factory = factory;
		}

		[Theory]
		[InlineData("/Shifts/")]
		[InlineData("/Shifts/Index")]
		[InlineData("/Shifts/Edit/1")]
		[InlineData("/Shifts/Delete/1")]
		[InlineData("/Shifts/Details/1")]
		[InlineData("/Shifts/AssignEmployee/1")]
		[InlineData("/Shifts/Create")]
		public async Task ShiftsControllerTest(string url)
		{
			HttpClient client = Factory.CreateClient();

			HttpResponseMessage response = await client.GetAsync(url);

			response.EnsureSuccessStatusCode();
			Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType.ToString());
		}
	}

	public class NotLogedInTests :
		IClassFixture<ShiftTestFactory<Startup>>
	{
		private readonly HttpClient Client;
		private readonly ShiftTestFactory<Startup> Factory;

		public NotLogedInTests(ShiftTestFactory<Startup> factory)
		{
			Factory = factory;
			Client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
		}

		[Theory]
		[InlineData("/Shifts/")]
		[InlineData("/Shifts/Index")]
		[InlineData("/Shifts/Edit/1")]
		[InlineData("/Shifts/Delete/1")]
		[InlineData("/Shifts/Details/1")]
		[InlineData("/Shifts/AssignEmployee/1")]
		[InlineData("/Shifts/Create")]
		public async Task ShiftAccesWhileNotLoggedIn(string url)
		{
			HttpResponseMessage Page = await Client.GetAsync(url);
			Assert.Equal(HttpStatusCode.Redirect, Page.StatusCode);
			Assert.StartsWith("http://localhost/Identity/Account/Login", Page.Headers.Location.OriginalString);
		}
	}

	public class LogedInTests :
		IClassFixture<ShiftTestFactory<Startup>>
	{
		private readonly HttpClient Client;
		private readonly ShiftTestFactory<Startup> Factory;

		public LogedInTests(ShiftTestFactory<Startup> factory)
		{
			Factory = factory;
			Client = factory.WithWebHostBuilder(builder => builder.ConfigureTestServices(services => services.AddAuthentication(Names.TestName).AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(Names.TestName, Options => { })))
				.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
			Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Names.TestName);
		}

		[Theory]
		[InlineData("/Shifts/")]
		[InlineData("/Shifts/Index")]
		[InlineData("/Shifts/Edit/1")]
		[InlineData("/Shifts/Delete/1")]
		[InlineData("/Shifts/Details/1")]
		[InlineData("/Shifts/AssignEmployee/1")]
		[InlineData("/Shifts/Create")]
		public async Task ShiftAccesToUnAllowedPlaces(string url)
		{
			HttpResponseMessage Page = await Client.GetAsync(url);
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
		[InlineData("/Shifts/")]
		[InlineData("/Shifts/Index")]
		[InlineData("/Shifts/Edit/1")]
		[InlineData("/Shifts/Delete/1")]
		[InlineData("/Shifts/Details/1")]
		[InlineData("/Shifts/AssignEmployee/1")]
		[InlineData("/Shifts/Create")]
		public async Task ShiftAccesToAllowedPlaces(string url)
		{
			HttpResponseMessage Page = await Client.GetAsync(url);
			Assert.Equal(HttpStatusCode.OK, Page.StatusCode);
		}

		[Theory]
		[InlineData("/Shifts/Edit/1")]
		[InlineData("/Shifts/Delete/1")]
		[InlineData("/Shifts/DeleteAllFollowing/1")]
		[InlineData("/Shifts/DeleteAllFollowing/2")]
		[InlineData("/Shifts/AssignEmployee/1")]
		[InlineData("/Shifts/Create")]
		public async Task NoDataTest(string url)
		{
			HttpResponseMessage Page = await Client.PostAsync(url, null);
			Assert.Equal(HttpStatusCode.BadRequest, Page.StatusCode);
		}

		[Theory]
		[InlineData(2)]
		public async Task DeleteAllFolowingTest(int id)
		{
			HttpResponseMessage defPage = await Client.GetAsync($"/Shifts/Delete/{id}");
			IHtmlDocument document = await HtmlHelpers.GetDocumentAsync(defPage);
			// makes the page ready to posted
			foreach ((string element, (string name, string value) attribute) data in Data.FormData["/Shifts/Delete/1"])
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
			Assert.Equal("/Shifts", response.Headers.Location.OriginalString);
		}

		[Theory]
		[InlineData("/Shifts/Edit/1")]
		[InlineData("/Shifts/Delete/1")]
		[InlineData("/Shifts/AssignEmployee/1")]
		[InlineData("/Shifts/Create")]
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
			Assert.Equal(url != "/Shifts/AssignEmployee/1" ? "/Shifts" : "/Shifts/Details/1", response.Headers.Location.OriginalString);
		}
	}

	public class ShiftTestFactory<TSartup> :
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
					ILogger<ShiftTestFactory<TSartup>> logger = ss.GetRequiredService<ILogger<ShiftTestFactory<TSartup>>>();
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