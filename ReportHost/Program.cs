using ReportHost;
using Serilog;
using ReportHost.Endpoints;
using ReportHost.Internal;

Log.Logger = new LoggerConfiguration().CreateBootstrapLogger();
var builder = WebApplication.CreateBuilder(args);
builder.AddConfiguration<Program>();
builder.AddApplicationLogging();
builder.AddServices();
builder.AddExceptionHandling();


var app = builder.Build();
app.UseReportHostExceptionHandler();
app.AddHealthEndpoint("/health");
app.AddVersionEndpoint("/version");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseSerilogRequestLogging();
app.UseHttpLogging();
app.UseExceptionHandler();

await app.RunAsync();