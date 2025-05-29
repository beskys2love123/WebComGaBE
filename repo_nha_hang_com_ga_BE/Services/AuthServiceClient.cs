namespace repo_nha_hang_com_ga_BE.Services;

public class AuthServiceClient
{
    private readonly HttpClient _httpClient;

    public AuthServiceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> ValidateTokenAsync(string token)
    {
        var response = await _httpClient.GetAsync($"/api/auth/validate?token={token}");
        return await response.Content.ReadAsStringAsync();
    }
}