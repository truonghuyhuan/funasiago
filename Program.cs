using Blazored.LocalStorage;
using FunAsiaGo;
using FunAsiaGo.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddCors();
builder.Services.AddHttpClient("WebApi", sp =>
{
    sp.BaseAddress = new Uri("https://funasiago.com");
    sp.Timeout = TimeSpan.FromMinutes(10);
});
builder.Services.AddScoped<IHttpService, HttpService>();
builder.Services.AddBlazoredLocalStorage();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
app.UseCors();

app.Run();
