
using Microsoft.EntityFrameworkCore;
using ReportHost.Data.Context;
using ReportHost.Internal;
using Serilog;

namespace ReportHost;

public static class Startup
{
    public static WebApplicationBuilder AddConfiguration<T>(this WebApplicationBuilder builder)
        where T : class
    {
        builder.Configuration.AddJsonFile("appsettings.json", true, true);
        builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true);
        builder.Configuration.AddJsonFile("/configuration/externalSecrets.json", true, true);
        builder.Configuration.AddJsonFile("/configuration/overrides.json", true, true);
        builder.Configuration.AddUserSecrets<T>();
        builder.Configuration.AddEnvironmentVariables();
        return builder;
    }

    public static WebApplicationBuilder AddServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddOptions();
        builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
        {
            options.SerializerOptions.AllowTrailingCommas = true;
            options.SerializerOptions.PropertyNameCaseInsensitive = true;
            options.SerializerOptions.WriteIndented = false;
            options.SerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
            options.SerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
        });
        builder.Services.AddDbContext<ReportContext>(opts =>
        {
            opts.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            opts.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
        });
        return builder;
    }

    public static void AddExceptionHandling(this WebApplicationBuilder builder)
    {
        //  Adds ASPNet Core AddProblemDetails
        //  Adds a ReportHostExceptionHandler
        //  Adds Custom exception handlers as defined in the builder
        builder.Services.AddReportHostExceptionHandling()
            .AddHandler<ConflictException>(StatusCodes.Status409Conflict, "Conflict", messageHandler: (e) => $"There was a conflict, {e.Message}", title: "Conflict")
            .AddHandler<UnauthorizedException>(StatusCodes.Status403Forbidden, "Forbidden", title: "Forbidden")
            .AddHandler<KeyNotFoundException>(StatusCodes.Status404NotFound, "NotFound", title: "Not Found")
            .AddHandler<NotFoundException>(StatusCodes.Status404NotFound, "NotFound", title: "Not Found")
            .Build();
    }

    public static void AddApplicationLogging(this WebApplicationBuilder builder)
    {
        builder.Services.AddLogging(c => c.ClearProviders());
        builder.Host.UseSerilog((hostBuilderContext, loggerConfiguration) =>
        {
            var useJsonLogging = hostBuilderContext.Configuration["USE_STANDARD_LOGGING"];
            loggerConfiguration
                .MinimumLevel.Override("Micorsoft", Serilog.Events.LogEventLevel.Warning)
                .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)
                .Enrich.WithProperty("env", builder.Environment.EnvironmentName)
                .Enrich.WithProperty("application", builder.Environment.ApplicationName);
            if (string.IsNullOrEmpty(useJsonLogging) == false)
                loggerConfiguration.WriteTo.Console();
            else
                loggerConfiguration.WriteTo.Console(new Serilog.Formatting.Compact.RenderedCompactJsonFormatter());
        }
        );
    }
}