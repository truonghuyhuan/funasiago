namespace FunAsiaGo;

public interface IHttpService
{
    public Task<T> GetAsync<T>(string endpoint);
    public Task<T> DeleteAsync<T>(string endpoint);
    public Task<T> PostAsync<T>(string endpoint, object data);
}