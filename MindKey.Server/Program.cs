using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MindKey.Server.Authorization;
using MindKey.Server.Helpers;
using MindKey.Server.Models;
using MindKey.Server.Services;
using MindKey.Server.Services.WordCloudGenerator;
using Quartz;
using SixLabors.ImageSharp.Web.Caching;
using SixLabors.ImageSharp.Web.DependencyInjection;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
SetupIOC(builder);
ConfigureBuilder(builder);
var app = builder.Build();
ConfigureApp(app);
app.Run();

static void SetupIOC(WebApplicationBuilder builder)
{
    builder.Services.AddScoped<IPersonRepository, PersonRepository>();
    builder.Services.AddScoped<IUploadRepository, UploadRepository>();
    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<IIdeaRepository, IdeaRepository>();
    builder.Services.AddScoped<IJwtUtils, JwtUtils>();

    builder.Services.AddScoped<WordCloudService>();
    builder.Services.AddScoped<IWordCloudGenerator, PythonWordCloudGenerator>();
}

static void ConfigureBuilder(WebApplicationBuilder builder)
{
    builder.Services.AddControllersWithViews();
    builder.Services.AddRazorPages();
    builder.Services.AddDbContextFactory<AppDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));
    builder.Services.AddImageSharp()
        .Configure<PhysicalFileSystemCacheOptions>(options =>
        {
            options.CacheFolder = "different-cache";
        });

    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "MindKey API",
            Version = "v1",
            Description = "MindKey API Services that act as the backend to the MindKey website."
        });

        // Set the comments path for the Swagger JSON and UI.
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        c.IncludeXmlComments(xmlPath);
        c.CustomSchemaIds(r => r.FullName);
    });

    builder.Services.AddQuartz(q =>
    {
        q.UseMicrosoftDependencyInjectionJobFactory();
        q.AddJobAndTrigger<UploadProcessorJob>(builder.Configuration);
    });
    builder.Services.AddQuartzHostedService(
        q => q.WaitForJobsToComplete = true);

    builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
    builder.Environment.ContentRootPath = Path.Combine(AppContext.BaseDirectory, "WebRoot");
    builder.Environment.WebRootPath = Path.Combine(AppContext.BaseDirectory, "WebRoot");
}

static void ConfigureApp(WebApplication app)
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;

        try
        {
            var appDbContext = services.GetRequiredService<IDbContextFactory<AppDbContext>>();
            DataGenerator.Initialize(appDbContext);
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred creating the DB.");
        }
    }

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseWebAssemblyDebugging();
    }
    else
    {
        app.UseExceptionHandler("/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHttpsRedirection();
        app.UseHsts();
    }

    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "mindkey.api v1");
        c.DefaultModelsExpandDepth(-1);
    });

    app.UseBlazorFrameworkFiles();
    app.UseStaticFiles();
    app.UseRouting();
    app.UseImageSharp();


    app.UseMiddleware<ErrorHandlerMiddleware>();
    app.UseMiddleware<JwtMiddleware>();

    app.MapRazorPages();
    app.MapControllers();
    app.MapFallbackToFile("index.html");
}