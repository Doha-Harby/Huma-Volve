
using Asp.Versioning;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services
    .AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = true;
        options.ApiVersionReader =
            new UrlSegmentApiVersionReader();
    })
    .AddMvc()
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Products API",
        Version = "v1"
    });

    options.SwaggerDoc("v2", new OpenApiInfo
    {
        Title = "Products API",
        Version = "v2"
    });

    options.DocInclusionPredicate((documentName, apiDescription) =>
        apiDescription.GroupName == documentName);
});

var app = builder.Build();

app.UseSwagger();

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint(
        "/swagger/v1/swagger.json",
        "Products API V1");

    options.SwaggerEndpoint(
        "/swagger/v2/swagger.json",
        "Products API V2");
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
