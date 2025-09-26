using System.Text.Json.Serialization;
using Internal.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureOpenTelemetry();
builder.ConfigureApiOptions();

builder.Services.AddHealthChecks();

builder.Services.AddInternalServices(builder.Configuration);

builder.Services.AddHttpLogging(options =>
{
    options.LoggingFields = HttpLoggingFields.Request |
                            HttpLoggingFields.Response;
});

builder.Services.AddControllers(options =>
{
    options.ReturnHttpNotAcceptable = true;
}).AddJsonOptions(options =>
{
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault | JsonIgnoreCondition.WhenWritingNull;
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
})
.ConfigureApiBehaviorOptions(options => options.SuppressModelStateInvalidFilter = true);

// NOTE: Uncomment for AzureAd authentication using JWT, commented out for brevity

//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));
//builder.Services.AddAuthorizationBuilder()
//    .SetDefaultPolicy(new AuthorizationPolicyBuilder()
//        .RequireAuthenticatedUser()
//        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
//        .Build());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger(c =>
    {
        c.PreSerializeFilters.Add((swagger, httpReq) =>
        {
            swagger.Servers = [new() { Url = $"{httpReq.Scheme}://{httpReq.Host.Value}" }];
        });
    });
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Vanguard Service");
        options.DisplayRequestDuration();
    });
}

app.UseRouting();
app.UseErrorHandler()
   .UseHttpLogging()
   .UseHttpsRedirection()

   // NOTE: Uncomment for AzureAd authentication using JWT, commented out for brevity
   //.UseAuthorization()
   //.UseAuthentication()
   //.Use(async (context, next) =>
   //{
   //    if (context.Request.Path == "/health" || (await context.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme)).Succeeded)
   //    {
   //        await next();
   //        return;
   //    }

   //    context.Response.StatusCode = 401;
   //})
   .UseResponseCaching()
   .UseResponseHeaders();

app.MapHealthChecks("/health");
app.MapControllers();

await app.RunAsync();