namespace FiveTalents.Tests.Integration.Members;

public class MemberWriteEndpointTests(IntegrationTestFactory factory) : IClassFixture<IntegrationTestFactory>
{
    private static object MinimalMember(string first = "Integration", string last = "Test") => new
    {
        OrganizationId = 1,
        FirstName = first,
        LastName = last,
        DateOfBirth = (DateTime?)null,
        Gender = (string?)null,
        MaritalStatus = (string?)null,
        JoinDate = (DateTime?)null,
        Addresses = (object?)null,
        Emails = (object?)null,
        Phones = (object?)null,
    };

    private async Task<int> CreateMemberAsync(HttpClient client, string first = "Test", string last = "Member")
    {
        HttpResponseMessage response = await client.PostAsJsonAsync("/api/members", MinimalMember(first, last));
        response.EnsureSuccessStatusCode();
        string location = response.Headers.Location!.ToString();
        return int.Parse(location.Split('/').Last());
    }

    [Fact]
    public async Task Update_WithValidData_Returns204()
    {
        HttpClient client = factory.CreateAuthenticatedClient();
        int id = await CreateMemberAsync(client, "Update", "Me");

        object body = new
        {
            Id = id,
            FirstName = "Updated",
            LastName = "Member",
            Status = "Active",
            DateOfBirth = (DateTime?)null,
            JoinDate = (DateTime?)null,
            Gender = (string?)null,
            MaritalStatus = (string?)null,
            Notes = (string?)null,
            Addresses = (object?)null,
            Emails = (object?)null,
            Phones = (object?)null,
        };

        HttpResponseMessage response = await client.PutAsJsonAsync($"/api/members/{id}", body);
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Update_WithNonMatchingId_Returns400()
    {
        HttpClient client = factory.CreateAuthenticatedClient();
        int id = await CreateMemberAsync(client, "Mismatch", "Test");

        object body = new
        {
            Id = id + 1,
            FirstName = "Wrong",
            LastName = "Id",
            Status = "Active",
        };

        HttpResponseMessage response = await client.PutAsJsonAsync($"/api/members/{id}", body);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Update_WithNonExistentId_Returns404()
    {
        HttpClient client = factory.CreateAuthenticatedClient();

        object body = new
        {
            Id = 99999,
            FirstName = "Ghost",
            LastName = "Member",
            Status = "Active",
        };

        HttpResponseMessage response = await client.PutAsJsonAsync("/api/members/99999", body);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_WithValidId_Returns204()
    {
        HttpClient client = factory.CreateAuthenticatedClient();
        int id = await CreateMemberAsync(client, "Delete", "Me");

        HttpResponseMessage response = await client.DeleteAsync($"/api/members/{id}");
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_WithNonExistentId_Returns404()
    {
        HttpClient client = factory.CreateAuthenticatedClient();

        HttpResponseMessage response = await client.DeleteAsync("/api/members/99999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_ThenGetById_Returns404()
    {
        HttpClient client = factory.CreateAuthenticatedClient();
        int id = await CreateMemberAsync(client, "Deleted", "User");

        await client.DeleteAsync($"/api/members/{id}");

        HttpResponseMessage getResponse = await client.GetAsync($"/api/members/{id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
