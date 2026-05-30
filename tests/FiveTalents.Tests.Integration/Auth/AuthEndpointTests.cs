namespace FiveTalents.Tests.Integration.Auth;

public class AuthEndpointTests(IntegrationTestFactory factory) : IClassFixture<IntegrationTestFactory>
{
    // The seeder creates admin@FiveTalents.local / Admin1234! on every startup.
    [Fact]
    public async Task Login_WithValidCredentials_Returns200WithToken()
    {
        HttpClient client = factory.CreateClient();
        object body = new { Email = "admin@FiveTalents.local", Password = "Admin1234!" };

        HttpResponseMessage response = await client.PostAsJsonAsync("/api/auth/login", body);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        string content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("token");
    }

    [Fact]
    public async Task Login_WithInvalidPassword_Returns401()
    {
        HttpClient client = factory.CreateClient();
        object body = new { Email = "admin@FiveTalents.local", Password = "WrongPassword!" };

        HttpResponseMessage response = await client.PostAsJsonAsync("/api/auth/login", body);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_WithUnknownEmail_Returns401()
    {
        HttpClient client = factory.CreateClient();
        object body = new { Email = "nobody@nowhere.com", Password = "Any1Password!" };

        HttpResponseMessage response = await client.PostAsJsonAsync("/api/auth/login", body);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Register_WithValidData_Returns200()
    {
        HttpClient client = factory.CreateClient();
        object body = new
        {
            FirstName = "Test",
            LastName = "User",
            Email = "newuser@integration.test",
            Password = "NewUser1234!",
        };

        HttpResponseMessage response = await client.PostAsJsonAsync("/api/auth/register", body);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Register_WithDuplicateEmail_Returns400()
    {
        HttpClient client = factory.CreateClient();
        object body = new
        {
            FirstName = "Dupe",
            LastName = "User",
            Email = "admin@FiveTalents.local",
            Password = "Duplicate1!",
        };

        HttpResponseMessage response = await client.PostAsJsonAsync("/api/auth/register", body);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
