namespace FiveTalents.Tests.Integration.Families;

public class FamiliesEndpointTests(IntegrationTestFactory factory) : IClassFixture<IntegrationTestFactory>
{
    [Fact]
    public async Task GetAll_WithAuth_Returns200()
    {
        HttpClient client = factory.CreateAuthenticatedClient();

        HttpResponseMessage response = await client.GetAsync("/api/families?organizationId=1");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetAll_WithoutAuth_Returns401()
    {
        HttpClient client = factory.CreateClient();

        HttpResponseMessage response = await client.GetAsync("/api/families?organizationId=1");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetById_WithNonExistentId_Returns404()
    {
        HttpClient client = factory.CreateAuthenticatedClient();

        HttpResponseMessage response = await client.GetAsync("/api/families/99999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_WithValidData_Returns201()
    {
        HttpClient client = factory.CreateAuthenticatedClient();
        object body = new { OrganizationId = 1, Name = "Smith Family" };

        HttpResponseMessage response = await client.PostAsJsonAsync("/api/families", body);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Create_ThenGetById_ReturnsFamily()
    {
        HttpClient client = factory.CreateAuthenticatedClient();
        object body = new { OrganizationId = 1, Name = "Jones Family" };

        HttpResponseMessage createResponse = await client.PostAsJsonAsync("/api/families", body);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        string location = createResponse.Headers.Location!.ToString();
        HttpResponseMessage getResponse = await client.GetAsync(location);
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Update_WithNonMatchingId_Returns400()
    {
        HttpClient client = factory.CreateAuthenticatedClient();

        HttpResponseMessage createResponse = await client.PostAsJsonAsync("/api/families", new { OrganizationId = 1, Name = "Brown Family" });
        string location = createResponse.Headers.Location!.ToString();
        int id = int.Parse(location.Split('/').Last());

        HttpResponseMessage response = await client.PutAsJsonAsync($"/api/families/{id}", new { Id = id + 1, Name = "Updated" });
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_ThenUpdate_Returns204()
    {
        HttpClient client = factory.CreateAuthenticatedClient();

        HttpResponseMessage createResponse = await client.PostAsJsonAsync("/api/families", new { OrganizationId = 1, Name = "Adams Family" });
        string location = createResponse.Headers.Location!.ToString();
        int id = int.Parse(location.Split('/').Last());

        HttpResponseMessage updateResponse = await client.PutAsJsonAsync($"/api/families/{id}", new { Id = id, Name = "Adams-Carter Family" });
        updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_WithNonExistentId_Returns404()
    {
        HttpClient client = factory.CreateAuthenticatedClient();

        HttpResponseMessage response = await client.DeleteAsync("/api/families/99999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_ThenDelete_Returns204()
    {
        HttpClient client = factory.CreateAuthenticatedClient();

        HttpResponseMessage createResponse = await client.PostAsJsonAsync("/api/families", new { OrganizationId = 1, Name = "White Family" });
        string location = createResponse.Headers.Location!.ToString();
        int id = int.Parse(location.Split('/').Last());

        HttpResponseMessage deleteResponse = await client.DeleteAsync($"/api/families/{id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
