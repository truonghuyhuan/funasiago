using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml;

namespace FunAsiaGo;

public interface IResult
{
    List<string> Messages { get; set; }

    bool Succeeded { get; set; }
}

public interface IResult<out T> : IResult
{
    T Data { get; }
}
public class Result : IResult
{
    public Result()
    {
    }

    public List<string> Messages { get; set; } = new List<string>();

    public bool Succeeded { get; set; }

    public static IResult Fail()
    {
        return new Result { Succeeded = false };
    }

    public static IResult Fail(string message)
    {
        return new Result { Succeeded = false, Messages = new List<string> { message } };
    }

    public static IResult Fail(List<string> messages)
    {
        return new Result { Succeeded = false, Messages = messages };
    }

    public static Task<IResult> FailAsync()
    {
        return Task.FromResult(Fail());
    }

    public static Task<IResult> FailAsync(string message)
    {
        return Task.FromResult(Fail(message));
    }

    public static Task<IResult> FailAsync(List<string> messages)
    {
        return Task.FromResult(Fail(messages));
    }

    public static IResult Success()
    {
        return new Result { Succeeded = true };
    }

    public static IResult Success(string message)
    {
        return new Result { Succeeded = true, Messages = new List<string> { message } };
    }

    public static Task<IResult> SuccessAsync()
    {
        return Task.FromResult(Success());
    }

    public static Task<IResult> SuccessAsync(string message)
    {
        return Task.FromResult(Success(message));
    }
}

public class Result<T> : Result, IResult<T>
{
    public T Data { get; set; } = default!;

    public new static Result<T> Fail()
    {
        return new Result<T> { Succeeded = false };
    }

    public new static Result<T> Fail(string message)
    {
        return new Result<T> { Succeeded = false, Messages = new List<string> { message } };
    }

    public new static Result<T> Fail(List<string> messages)
    {
        return new Result<T> { Succeeded = false, Messages = messages };
    }

    public static Result<T> Fail(T data)
    {
        return new Result<T> { Succeeded = false, Data = data };
    }

    public static Task<Result<T>> FailAsync(T data)
    {
        return Task.FromResult(Fail(data));
    }

    public new static Task<Result<T>> FailAsync()
    {
        return Task.FromResult(Fail());
    }

    public new static Task<Result<T>> FailAsync(string message)
    {
        return Task.FromResult(Fail(message));
    }

    public new static Task<Result<T>> FailAsync(List<string> messages)
    {
        return Task.FromResult(Fail(messages));
    }

    public new static Result<T> Success()
    {
        return new Result<T> { Succeeded = true };
    }

    public new static Result<T> Success(string message)
    {
        return new Result<T> { Succeeded = true, Messages = new List<string> { message } };
    }

    public static Result<T> Success(T data)
    {
        return new Result<T> { Succeeded = true, Data = data };
    }

    public static Result<T> Success(T data, string message)
    {
        return new Result<T> { Succeeded = true, Data = data, Messages = new List<string> { message } };
    }

    public static Result<T> Success(T data, List<string> messages)
    {
        return new Result<T> { Succeeded = true, Data = data, Messages = messages };
    }

    public new static Task<Result<T>> SuccessAsync()
    {
        return Task.FromResult(Success());
    }

    public new static Task<Result<T>> SuccessAsync(string message)
    {
        return Task.FromResult(Success(message));
    }

    public static Task<Result<T>> SuccessAsync(T data)
    {
        return Task.FromResult(Success(data));
    }

    public static Task<Result<T>> SuccessAsync(T data, string message)
    {
        return Task.FromResult(Success(data, message));
    }
}

public static class Extensions
{
    public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> source)
    {
        return source.Select((item, index) => (item, index));
    }
}

public static class HttpClientExtensions
{
    public static async Task<(string data, HttpStatusCode statusCode)> GetHeadersReadAsync(this HttpClient httpClient,
        string url)
    {
        using var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
        if (response.StatusCode != HttpStatusCode.OK)
        {
            return ("", response.StatusCode);
        }

        var content = await response.Content.ReadAsStringAsync();
        return (content, HttpStatusCode.OK);
    }

    public static async Task<string> GetHeadersReadNoStatusAsync(this HttpClient httpClient, string url)
    {
        using var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        return content;
    }

    public static async Task<(string data, HttpStatusCode statusCode)> GetHeadersReadWithStatusAsync(
        this HttpClient httpClient, string url)
    {
        using var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
        var content = await response.Content.ReadAsStringAsync();
        return (content, response.StatusCode);
    }

    public static async Task<(string data, HttpStatusCode statusCode)> PostHeadersReadAsync(this HttpClient httpClient,
        string url, object data, JsonSerializerOptions? jsonOptions = null)
    {
        var ms = new MemoryStream();
        var json = JsonSerializer.Serialize(data, jsonOptions);
        await JsonSerializer.SerializeAsync(ms, data, jsonOptions);
        ms.Seek(0, SeekOrigin.Begin);
        var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        using var requestContent = new StreamContent(ms);
        request.Content = requestContent;
        requestContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        try
        {
            using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                return ("", response.StatusCode);
            }

            var content = await response.Content.ReadAsStreamAsync();
            var reader = new StreamReader(content);
            var result = await reader.ReadToEndAsync();
            return (result, HttpStatusCode.OK);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return ("", HttpStatusCode.OK);
        }
    }

    public static async Task<string> PostHeadersAsync<T>(this HttpClient httpClient, string url, T data,
        JsonSerializerOptions? jsonOptions = null)
    {
        var ms = new MemoryStream();
        await JsonSerializer.SerializeAsync(ms, data, jsonOptions);
        ms.Seek(0, SeekOrigin.Begin);
        var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        using var requestContent = new StreamContent(ms);
        request.Content = requestContent;
        requestContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        if (response.StatusCode == HttpStatusCode.InternalServerError) return "";
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStreamAsync();
        var reader = new StreamReader(content);
        var result = await reader.ReadToEndAsync();
        return result;
    }

    public static async Task<HttpResponseMessage> PostHeadersNewAsync(this HttpClient httpClient, string url,
        object data, JsonSerializerOptions? jsonOptions = null)
    {
        var ms = new MemoryStream();
        var json = JsonSerializer.Serialize(data, jsonOptions);
        await JsonSerializer.SerializeAsync(ms, data, jsonOptions);
        ms.Seek(0, SeekOrigin.Begin);
        var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        using var requestContent = new StreamContent(ms);
        request.Content = requestContent;
        requestContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        try
        {
            return await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new HttpResponseMessage(HttpStatusCode.InternalServerError);
        }
    }

    public static async Task<string> PutHeadersAsync<T>(this HttpClient httpClient, string url, T data,
        JsonSerializerOptions? jsonOptions = null)
    {
        var ms = new MemoryStream();
        await JsonSerializer.SerializeAsync(ms, data, jsonOptions);
        ms.Seek(0, SeekOrigin.Begin);
        var request = new HttpRequestMessage(HttpMethod.Put, url);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        using var requestContent = new StreamContent(ms);
        request.Content = requestContent;
        requestContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        if (response.StatusCode == HttpStatusCode.InternalServerError) return "";
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStreamAsync();
        var reader = new StreamReader(content);
        var result = await reader.ReadToEndAsync();
        return result;
    }
}

public static class StringExtensions
{
    public static bool IsNullOrEmpty(this string? s)
    {
        return string.IsNullOrEmpty(s);
    }

    public static bool NotIsNullOrEmpty(this string? s)
    {
        return !string.IsNullOrEmpty(s);
    }
}
public static class ResultExtensions
{
    public static async Task<IResult<T>> ToResult<T>(this HttpResponseMessage response)
    {
        var responseAsString = await response.Content.ReadAsStringAsync();
        if (responseAsString.IsNullOrEmpty()) return Activator.CreateInstance<IResult<T>>();
        var responseObject = JsonSerializer.Deserialize<Result<T>>(responseAsString, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReferenceHandler = ReferenceHandler.Preserve
        });
        return responseObject!;
    }

    public static IResult<T> ToResult<T>(this string source)
    {
        if (source.IsNullOrEmpty()) return Activator.CreateInstance<IResult<T>>();
        var responseObject = JsonSerializer.Deserialize<Result<T>>(source, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReferenceHandler = ReferenceHandler.Preserve
        });
        return responseObject!;
    }

    public static async Task<T> ToObject<T>(this HttpResponseMessage response, bool nameCaseInsensitive = true)
    {
        var responseAsString = await response.Content.ReadAsStringAsync();
        if (responseAsString.IsNullOrEmpty()) return Activator.CreateInstance<T>();
        try
        {
            var responseObject = JsonSerializer.Deserialize<T>(responseAsString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = nameCaseInsensitive,
                ReferenceHandler = ReferenceHandler.Preserve
            });
            return responseObject!;
        }
        catch
        {
            return Activator.CreateInstance<T>();
        }
    }
    public static async Task<T> ToObject<T>(this HttpResponseMessage response, JsonSerializerOptions options)
    {
        var responseAsString = await response.Content.ReadAsStringAsync();
        if (responseAsString.IsNullOrEmpty()) return Activator.CreateInstance<T>();
        try
        {
            var responseObject = JsonSerializer.Deserialize<T>(responseAsString, options);
            return responseObject!;
        }
        catch
        {
            return Activator.CreateInstance<T>();
        }
    }

    public static T ToObject<T>(this string response, bool nameCaseInsensitive = true)
    {
        if (response.IsNullOrEmpty()) return Activator.CreateInstance<T>();
        try
        {
            var responseObject = JsonSerializer.Deserialize<T>(response, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = nameCaseInsensitive,
                ReferenceHandler = ReferenceHandler.Preserve
            });
            return responseObject!;
        }
        catch
        {
            return Activator.CreateInstance<T>();
        }
    }
    public static T ToObject<T>(this string response,JsonSerializerOptions options)
    {
        if (response.IsNullOrEmpty()) return Activator.CreateInstance<T>();
        try
        {
            var responseObject = JsonSerializer.Deserialize<T>(response, options);
            return responseObject!;
        }
        catch
        {
            return Activator.CreateInstance<T>();
        }
    }

    public static async Task<IResult> ToResult(this HttpResponseMessage response)
    {
        var responseAsString = await response.Content.ReadAsStringAsync();
        if (responseAsString.IsNullOrEmpty()) return Activator.CreateInstance<IResult>();
        var responseObject = JsonSerializer.Deserialize<Result>(responseAsString, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReferenceHandler = ReferenceHandler.Preserve
        });
        return responseObject!;
    }

    public static IResult ToResult(this string response)
    {
        if (response.IsNullOrEmpty()) return Activator.CreateInstance<IResult>();
        var responseObject = JsonSerializer.Deserialize<Result>(response, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReferenceHandler = ReferenceHandler.Preserve
        });
        return responseObject!;
    }
    
    private static readonly JsonSerializerOptions SWriteOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = false,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
    };
}