using RichardSzalay.MockHttp;
using Xunit;
using static FluentlyHttpClient.Test.ServiceTestUtil;

namespace FluentlyHttpClient.Test
{
	public class FluentHttpRequestExtensionsTest
	{
		[Fact]
		public async void FluentHttpRequest_Send_ShouldReturnContent()
		{
			var mockHttp = new MockHttpMessageHandler();
			mockHttp.When("https://sketch7.com/api/heroes/azmodan")
				.Respond("application/json", "{ 'name': 'Azmodan' }");


			var fluentHttpClientFactory = GetNewClientFactory();
			var clientBuilder = fluentHttpClientFactory.CreateBuilder("sketch7")
				.WithBaseUrl("https://sketch7.com")
				.WithMessageHandler(mockHttp);

			var httpClient = fluentHttpClientFactory.Add(clientBuilder);
			var fluentHttpRequest = new FluentHttpRequestBuilder(httpClient).WithUri("api/heroes/azmodan").Build();
			var hero = await fluentHttpRequest.SendAsync<Hero>();

			Assert.NotNull(hero);
			Assert.Equal("Azmodan", hero.Name);
		}

		[Fact]
		public async void FluentHttpRequestBuilder_Send_ShouldReturnContent()
		{
			var mockHttp = new MockHttpMessageHandler();
			mockHttp.When("https://sketch7.com/api/heroes/azmodan")
				.Respond("application/json", "{ 'name': 'Azmodan' }");


			var fluentHttpClientFactory = GetNewClientFactory();
			var clientBuilder = fluentHttpClientFactory.CreateBuilder("sketch7")
				.WithBaseUrl("https://sketch7.com")
				.WithMessageHandler(mockHttp);

			var httpClient = fluentHttpClientFactory.Add(clientBuilder);
			var hero = await new FluentHttpRequestBuilder(httpClient).WithUri("api/heroes/azmodan").SendAsync<Hero>();

			Assert.NotNull(hero);
			Assert.Equal("Azmodan", hero.Name);
		}
	}
}