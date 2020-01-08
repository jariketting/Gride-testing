using Gride;
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
		public class EmployeeControllerTest : IClassFixture<WebApplicationFactory<Startup>>
		{
			private readonly WebApplicationFactory<Startup> Factory;

			public EmployeeControllerTest(WebApplicationFactory<Startup> factory)
			{
				Factory = factory;
			}

			[Theory]
			[InlineData("/Shift/Generate/")]
			[InlineData("/Shift/Generated/")]
			public async Task EmployeeControllerReturnsViewTest(string url)
			{
				HttpClient client = Factory.CreateClient();

				HttpResponseMessage response = await client.GetAsync(url);

				response.EnsureSuccessStatusCode();
				Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType.ToString());
			}
		}

		
	}
}
