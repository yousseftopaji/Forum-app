using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using DTOs.Models.Login;
using DTOs.Models;
using System.Text.Json;
using Microsoft.JSInterop;


namespace BlazorApp1.Auth;

public class SimpleAuthProvider : AuthenticationStateProvider
{
    private readonly HttpClient httpClient;
    private readonly IJSRuntime jSRuntime;

    public SimpleAuthProvider(HttpClient httpClient, IJSRuntime jSRuntime)
    {
        this.httpClient = httpClient;
        this.jSRuntime = jSRuntime;
    }

    public async Task Login(string username, string password)
    {
        DTOs.Models.Login.LoginRequest loginRequest = new() { Username = username, Password = password };
        HttpResponseMessage response =
            await httpClient.PostAsJsonAsync("auth/login", loginRequest);
        string content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(content);
        }
        UserDTO userDTO = JsonSerializer.Deserialize<UserDTO>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        string serializedData = JsonSerializer.Serialize(userDTO);
        await jSRuntime.InvokeVoidAsync("sessionStorage.setItem", "currentUser", serializedData);

        List<Claim> claims =
        [
            new Claim(ClaimTypes.Name, userDTO.Username),
            new Claim(ClaimTypes.NameIdentifier, userDTO.Id.ToString()),
        ];
        ClaimsIdentity claimsIdentity = new(claims, "apiauth");
        ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        string userAsJson = "";
        try
        {
            userAsJson = await jSRuntime.InvokeAsync<string>("sessionStorage.getItem", "currentUser");
        }
        catch (InvalidOperationException e)
        {
            return new AuthenticationState(new());
        }

        if (string.IsNullOrEmpty(userAsJson))
        {
            return new AuthenticationState(new());
        }

        UserDTO userDTO = JsonSerializer.Deserialize<UserDTO>(userAsJson)!;
        List<Claim> claims =
        [
            new Claim(ClaimTypes.Name, userDTO.Username),
            new Claim(ClaimTypes.NameIdentifier, userDTO.Id.ToString()),
        ];

        ClaimsIdentity identity = new ClaimsIdentity(claims, "apiauth");
        ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);
        return new AuthenticationState(claimsPrincipal);
    }

    public async Task Logout()
    {
        await jSRuntime.InvokeVoidAsync("sessionStorage.setItem", "currentUser", "");
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new ClaimsPrincipal())));
    }
}