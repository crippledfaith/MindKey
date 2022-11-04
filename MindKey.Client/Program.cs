using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MindKey.Client;
using MindKey.Client.Services;
using MindKey.Client.Shared;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddLogging(builder => builder
    .SetMinimumLevel(LogLevel.Trace)
);


builder.Services.AddScoped<IIdeaService, IdeaService>();
builder.Services.AddScoped<IPersonService, PersonService>();
builder.Services.AddScoped<IUploadService, UploadService>();
builder.Services.AddScoped<IAlertService, AlertService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IHttpService, HttpService>();
builder.Services.AddScoped<ILocalStorageService, LocalStorageService>();
builder.Services.AddSingleton<EventService>();

builder.Services.AddECharts();

builder.Services.AddScoped(x =>
{
    var apiUrl = new Uri("http://localhost:5001");
    return new HttpClient() { BaseAddress = apiUrl };
});
builder.Services.AddSingleton<PageHistoryState>();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();