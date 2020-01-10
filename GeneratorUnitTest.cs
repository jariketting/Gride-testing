using Gride;
using Gride.Controllers;
using Gride.Data;
using Gride.Gen;
using Gride.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace GrideTest
{
    public class GeneratorUnitTest
    {
		public static readonly Dictionary<string, List<(string element, (string name, string value) attribute)>> FormData = new Dictionary<string, List<(string element, (string name, string value) attribute)>>
		{
			{ "/Shift/Generate", new List<(string element, (string name, string value) attribute)>
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

		public class EmployeeControllerTest : IClassFixture<WebApplicationFactory<Startup>>
		{
			private readonly WebApplicationFactory<Startup> Factory;

			public EmployeeControllerTest(WebApplicationFactory<Startup> factory)
			{
				Factory = factory;
			}

			[Theory]
			[InlineData("/Shifts/Generate/")]
			[InlineData("/Shifts/Generated/")]
			public async Task GeneratorControllerReturnsViewTest(string url)
			{
				HttpClient client = Factory.CreateClient();

				HttpResponseMessage response = await client.GetAsync(url);

				response.EnsureSuccessStatusCode();
				Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType.ToString());
			}
		}

		
	}
}