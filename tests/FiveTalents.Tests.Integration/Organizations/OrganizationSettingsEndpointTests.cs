namespace FiveTalents.Tests.Integration.Organizations;

public class OrganizationSettingsEndpointTests(IntegrationTestFactory factory) : IClassFixture<IntegrationTestFactory>
{
    [Fact]
    public async Task GetSettings_ForSeededOrg_Returns200()
    {
        HttpClient client = factory.CreateAuthenticatedClient();

        HttpResponseMessage response = await client.GetAsync("/api/organizations/1/settings");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateSettings_WithValidData_Returns204()
    {
        HttpClient client = factory.CreateAuthenticatedClient();
        object body = new
        {
            OrganizationId = 1,
            Currency = "USD",
            FiscalYearStart = "01-01",
            EnableAttendanceTracking = true,
            EnableMemberPortal = false,
            EnableOnlineGiving = false,
            GoogleWorkspaceEnabled = false,
        };

        HttpResponseMessage response = await client.PutAsJsonAsync("/api/organizations/1/settings", body);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task GetTree_Returns200()
    {
        HttpClient client = factory.CreateAuthenticatedClient();

        HttpResponseMessage response = await client.GetAsync("/api/organizations/tree");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetLevels_Returns200()
    {
        HttpClient client = factory.CreateAuthenticatedClient();

        HttpResponseMessage response = await client.GetAsync("/api/organizations/levels");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Update_WithValidData_Returns204()
    {
        HttpClient client = factory.CreateAuthenticatedClient();
        object body = new
        {
            Id = 1,
            Name = "My Church (Updated)",
            Level = 3,
            IsActive = true,
        };

        HttpResponseMessage response = await client.PutAsJsonAsync("/api/organizations/1", body);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
