namespace FiveTalents.Tests.Integration.Groups;

public class GroupsEndpointTests(IntegrationTestFactory factory) : IClassFixture<IntegrationTestFactory>
{
    // The seeder creates 7 default group types; GroupTypeId=1 is always available.
    private static object MinimalGroup(string name = "Test Group") => new
    {
        OrganizationId = 1,
        Name = name,
        Description = (string?)null,
        GroupTypeId = 1,
        Status = "Active",
        LeaderMemberId = (int?)null,
        CoLeaderMemberId = (int?)null,
        MeetingFrequency = (string?)null,
        MeetingDay = (string?)null,
        MeetingTime = (string?)null,
        MeetingLocation = (string?)null,
        MaxCapacity = (int?)null,
        IsOpenToNewMembers = true,
        ImageUrl = (string?)null,
    };

    [Fact]
    public async Task GetAll_WithAuth_Returns200()
    {
        HttpClient client = factory.CreateAuthenticatedClient();

        HttpResponseMessage response = await client.GetAsync("/api/groups?organizationId=1");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetAll_WithoutAuth_Returns401()
    {
        HttpClient client = factory.CreateClient();

        HttpResponseMessage response = await client.GetAsync("/api/groups?organizationId=1");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetTypes_WithAuth_Returns200()
    {
        HttpClient client = factory.CreateAuthenticatedClient();

        HttpResponseMessage response = await client.GetAsync("/api/groups/types?organizationId=1");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetById_WithNonExistentId_Returns404()
    {
        HttpClient client = factory.CreateAuthenticatedClient();

        HttpResponseMessage response = await client.GetAsync("/api/groups/99999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_WithValidData_Returns201()
    {
        HttpClient client = factory.CreateAuthenticatedClient();

        HttpResponseMessage response = await client.PostAsJsonAsync("/api/groups", MinimalGroup());

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Create_ThenGetById_ReturnsGroup()
    {
        HttpClient client = factory.CreateAuthenticatedClient();

        HttpResponseMessage createResponse = await client.PostAsJsonAsync("/api/groups", MinimalGroup("Bible Study"));
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        string location = createResponse.Headers.Location!.ToString();
        HttpResponseMessage getResponse = await client.GetAsync(location);
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Update_WithNonMatchingId_Returns400()
    {
        HttpClient client = factory.CreateAuthenticatedClient();

        HttpResponseMessage createResponse = await client.PostAsJsonAsync("/api/groups", MinimalGroup());
        string location = createResponse.Headers.Location!.ToString();
        int id = int.Parse(location.Split('/').Last());

        object mismatchedBody = new
        {
            Id = id + 1,
            Name = "Updated",
            GroupTypeId = 1,
            Status = "Active",
            IsOpenToNewMembers = true,
        };

        HttpResponseMessage response = await client.PutAsJsonAsync($"/api/groups/{id}", mismatchedBody);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Delete_WithNonExistentId_Returns404()
    {
        HttpClient client = factory.CreateAuthenticatedClient();

        HttpResponseMessage response = await client.DeleteAsync("/api/groups/99999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_ThenDelete_Returns204()
    {
        HttpClient client = factory.CreateAuthenticatedClient();

        HttpResponseMessage createResponse = await client.PostAsJsonAsync("/api/groups", MinimalGroup());
        string location = createResponse.Headers.Location!.ToString();
        int id = int.Parse(location.Split('/').Last());

        HttpResponseMessage deleteResponse = await client.DeleteAsync($"/api/groups/{id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
