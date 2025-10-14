using Microsoft.AspNetCore.Components;
using System.Net;

namespace FunAsiaGo;

public class HttpService(IHttpClientFactory clientFactory, NavigationManager navigationManager) : IHttpService
{
    public async Task<T> GetAsync<T>(string endpoint)
    {
        var client = clientFactory.CreateClient("WebApi");
        var response = await client.GetHeadersReadAsync(endpoint);
        switch (response.statusCode)
        {
            case HttpStatusCode.OK:
                return response.data.ToObject<T>()!;
            case HttpStatusCode.Unauthorized:
            case HttpStatusCode.Redirect:
                navigationManager.NavigateTo("Account/Login", forceLoad: true);
                break;
        }

        return default!;
    }

    public async Task<T> DeleteAsync<T>(string endpoint)
    {
        var client = clientFactory.CreateClient("WebApi");
        var response = await client.DeleteAsync(endpoint);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        return content.ToObject<T>()!;
    }

    public async Task<T> PostAsync<T>(string endpoint, object data)
    {
        var client = clientFactory.CreateClient("WebApi");
        var response = await client.PostHeadersReadAsync(endpoint, data);
        switch (response.statusCode)
        {
            case HttpStatusCode.OK:
                return response.data.ToObject<T>()!;
            case HttpStatusCode.Unauthorized:
            case HttpStatusCode.Redirect:
                navigationManager.NavigateTo("Account/Login", forceLoad: true);
                break;
        }

        return default!;
    }
}