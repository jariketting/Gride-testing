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
	internal static class EmployeeData
	{
		public static readonly Dictionary<string, List<(string element, (string name, string value) attribute)>> FormData = new Dictionary<string, List<(string element, (string name, string value) attribute)>>
		{
			{ "/Employee/Edit/1", new List<(string element, (string name, string value) attribute)>
			{
				(@".//input[@id=""ProfileImage""]",("value","img.jpg")),
				(@".//select[@name=""SupervisorID""]",("value","1")),
			}},
			{ "/Employee/Create", new List<(string element, (string name, string value) attribute)>
			{
				(@".//input[@id=""Name""]",("value","First1")),
				(@".//input[@id=""LastName""]",("value","Last1")),
				(@".//input[@id=""DoB""]",("value","18/05/1998")),
				(@".//input[@id=""EMail""]",("value","gj@gmail.com")),
				(@".//input[@id=""PhoneNumber""]",("value","0612345678")),
				(@".//input[@id=""Experience""]",("value","1")),
				(@".//input[@id=""ProfileImage""]",("value","img.jpg")),
				(@".//select[@name=""SupervisorID""]",("value","1")),
			}},
			// delete has no data for us to set
			{ "/Employee/Delete/1", new List<(string element, (string name, string value) attribute)>()},
		};
	}

	public class EmployeeControllerTest : IClassFixture<WebApplicationFactory<Startup>>
	{
		private readonly WebApplicationFactory<Startup> Factory;

		public EmployeeControllerTest(WebApplicationFactory<Startup> factory)
		{
			Factory = factory;
		}

		[Theory]
		[InlineData("/Employee/")]
		[InlineData("/Employee/Index")]
		[InlineData("/Employee/Edit/1")]
		[InlineData("/Employee/Delete/1")]
		[InlineData("/Employee/Details/1")]
		[InlineData("/Employee/Create")]
		public async Task EmployeeControllerReturnsViewTest(string url)
		{
			HttpClient client = Factory.CreateClient();

			HttpResponseMessage response = await client.GetAsync(url);

			response.EnsureSuccessStatusCode();
			Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType.ToString());
		}
	}

	public class RedirectNotLoggedInUsersEmployeePagesTest :
		IClassFixture<ShiftTestFactory<Startup>>
	{
		private readonly HttpClient Client;
		private readonly ShiftTestFactory<Startup> Factory;

		public RedirectNotLoggedInUsersEmployeePagesTest(ShiftTestFactory<Startup> factory)
		{
			Factory = factory;
			Client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
		}

		[Theory]
		[InlineData("/Employee/")]
		[InlineData("/Employee/Index")]
		[InlineData("/Employee/Edit/1")]
		[InlineData("/Employee/Delete/1")]
		[InlineData("/Employee/Details/1")]
		[InlineData("/Employee/Create")]
		public async Task RedirectNotLoggedInUsersFromEmployeeAccess(string url)
		{
			HttpResponseMessage Page = await Client.GetAsync(url);
			Assert.Equal(HttpStatusCode.Redirect, Page.StatusCode);
			Assert.StartsWith("http://localhost/Identity/Account/Login", Page.Headers.Location.OriginalString);
		}
	}

	public class RestrictNotAuthorisedUsersEmployeePagesTest :
		IClassFixture<ShiftTestFactory<Startup>>
	{
		private readonly HttpClient Client;
		private readonly ShiftTestFactory<Startup> Factory;

		public RestrictNotAuthorisedUsersEmployeePagesTest(ShiftTestFactory<Startup> factory)
		{
			Factory = factory;
			Client = factory.WithWebHostBuilder(builder => builder.ConfigureTestServices(services => services.AddAuthentication(Names.TestName).AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(Names.TestName, Options => { })))
				.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
			Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Names.TestName);
		}

		[Theory]
		[InlineData("/Employee/")]
		[InlineData("/Employee/Index")]
		[InlineData("/Employee/Edit/1")]
		[InlineData("/Employee/Delete/1")]
		[InlineData("/Employee/Details/1")]
		[InlineData("/Employee/Create")]
		public async Task RestrictAccessNotAuthorisedUsersEmployeePages(string url)
		{
			HttpResponseMessage Page = await Client.GetAsync(url);
			Assert.Equal(HttpStatusCode.Forbidden, Page.StatusCode);
		}
	}

	public class AllowAuthorisedUsersEmployeePagesTest :
		IClassFixture<ShiftTestFactory<Startup>>
	{
		private readonly ShiftTestFactory<Startup> Factory;
		private readonly HttpClient Client;

		public AllowAuthorisedUsersEmployeePagesTest(ShiftTestFactory<Startup> factory)
		{
			Factory = factory;
			Client = Factory.WithWebHostBuilder(builder => builder.ConfigureTestServices(services => services.AddAuthentication(Names.AdminTestName).AddScheme<AuthenticationSchemeOptions, AdminTestAuthHandler>(Names.AdminTestName, Options => { })))
				.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
			Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Names.AdminTestName);
		}

		[Theory]
		[InlineData("/Employee/")]
		[InlineData("/Employee/Index")]
		[InlineData("/Employee/Edit/1")]
		[InlineData("/Employee/Delete/1")]
		[InlineData("/Employee/Details/1")]
		[InlineData("/Employee/Create")]
		public async Task AllowAccessAuthorisedUsersEmployeePages(string url)
		{
			HttpResponseMessage Page = await Client.GetAsync(url);
			Assert.Equal(HttpStatusCode.OK, Page.StatusCode);
		}


		[Theory]
		[InlineData("/Employee/Edit/1")]
		[InlineData("/Employee/Delete/1")]
		[InlineData("/Employee/Create")]
		public async Task EmployeeFormsWork(string url)
		{
			HttpResponseMessage defPage = await Client.GetAsync(url);
			IHtmlDocument document = await HtmlHelpers.GetDocumentAsync(defPage);
			// makes the page ready to be posted
			foreach ((string element, (string name, string value) attribute) data in EmployeeData.FormData[url])
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
			IHtmlInputElement submit = (IHtmlInputElement)form.SelectSingleNode(@".//input[@type=""submit""]");
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

	public class EmployeeTestFactory<TSartup> :
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
}