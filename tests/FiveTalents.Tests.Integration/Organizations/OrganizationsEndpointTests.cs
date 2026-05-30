namespace FiveTalents.Tests.Integration.Organizations;

public class OrganizationsEndpointTests(IntegrationTestFactory factory) : IClassFixture<IntegrationTestFactory>
{
    [Fact]
    public async Task GetAll_WithAuth_Returns200()
    {
        HttpClient client = factory.CreateAuthenticatedClient();

        HttpResponseMessage response = await client.GetAsync("/api/organizations");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetAll_WithoutAuth_Returns401()
    {
        HttpClient client = factory.CreateClient();

        HttpResponseMessage response = await client.GetAsync("/api/organizations");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetAll_ReturnsJsonArray()
    {
        HttpClient client = factory.CreateAuthenticatedClient();

        HttpResponseMessage response = await client.GetAsync("/api/organizations");
        List<object>? orgs = await response.Content.ReadFromJsonAsync<List<object>>();

        orgs.Should().NotBeNull();
    }

    [Fact]
    public async Task GetById_WithNonExistentId_Returns404()
    {
        HttpClient client = factory.CreateAuthenticatedClient();

        HttpResponseMessage response = await client.GetAsync("/api/organizations/99999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetById_WithSeededOrg_Returns200()
    {
        HttpClient client = factory.CreateAuthenticatedClient();

        // The seeder always creates an org with Id=1
        HttpResponseMessage response = await client.GetAsync("/api/organizations/1");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Create_WithValidData_Returns201()
    {
        HttpClient client = factory.CreateAuthenticatedClient();
        object body = new { Name = "Integration Test Diocese", Level = 1 };

        HttpResponseMessage response = await client.PostAsJsonAsync("/api/organizations", body);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Create_WithEmptyName_Returns400()
    {
        HttpClient client = factory.CreateAuthenticatedClient();
        object body = new { Name = "", Level = 1 };

        HttpResponseMessage response = await client.PostAsJsonAsync("/api/organizations", body);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_WithInvalidEmail_Returns400()
    {
        HttpClient client = factory.CreateAuthenticatedClient();
        object body = new { Name = "Test Org", Level = 1, Email = "not-an-email" };

        HttpResponseMessage response = await client.PostAsJsonAsync("/api/organizations", body);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
