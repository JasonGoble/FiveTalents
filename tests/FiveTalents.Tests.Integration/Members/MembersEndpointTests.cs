namespace FiveTalents.Tests.Integration.Members;

public class MembersEndpointTests(IntegrationTestFactory factory) : IClassFixture<IntegrationTestFactory>
{
    [Fact]
    public async Task GetAll_WithoutAuth_Returns401()
    {
        HttpClient client = factory.CreateClient();

        HttpResponseMessage response = await client.GetAsync("/api/members?organizationId=1");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetAll_WithAuth_Returns200()
    {
        HttpClient client = factory.CreateAuthenticatedClient();

        HttpResponseMessage response = await client.GetAsync("/api/members?organizationId=1");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetById_WithNonExistentId_Returns404()
    {
        HttpClient client = factory.CreateAuthenticatedClient();

        HttpResponseMessage response = await client.GetAsync("/api/members/99999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_WithValidData_Returns201()
    {
        HttpClient client = factory.CreateAuthenticatedClient();
        object body = new
        {
            OrganizationId = 1,
            FirstName = "Integration",
            LastName = "Test",
            DateOfBirth = (DateTime?)null,
            Gender = (string?)null,
            MaritalStatus = (string?)null,
            JoinDate = (DateTime?)null,
            Addresses = (object?)null,
            Emails = (object?)null,
            Phones = (object?)null,
        };

        HttpResponseMessage response = await client.PostAsJsonAsync("/api/members", body);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Create_ThenGetById_ReturnsMember()
    {
        HttpClient client = factory.CreateAuthenticatedClient();
        object body = new
        {
            OrganizationId = 1,
            FirstName = "Jane",
            LastName = "Retrieval",
            DateOfBirth = (DateTime?)null,
            Gender = (string?)null,
            MaritalStatus = (string?)null,
            JoinDate = (DateTime?)null,
            Addresses = (object?)null,
            Emails = (object?)null,
            Phones = (object?)null,
        };

        HttpResponseMessage createResponse = await client.PostAsJsonAsync("/api/members", body);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        string location = createResponse.Headers.Location!.ToString();
        HttpResponseMessage getResponse = await client.GetAsync(location);
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
