using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    /* 
     * If I don't use the following filter, endpoint parameters of type path, query and header,
     * when an endpoint contains also a body parameter, have a "content" property
     * with an "application/json" field, that totally messes the definition, for example:
     * {
     *     "name": "id",
     *     "in": "path",
     *     "required": true,
     *     "style": "simple",
     *     "schema": {
     *          "type": "string"
     *     },
     *     "content": {                    <-- THIS IS INCORRECT
     *          "application/json": { }
     *     }
     * }
    */
    options.OperationFilter<ResetContentOperationFilter>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    _ = app.UseSwagger();
    _ = app.UseSwaggerUI();
}

app.MapPost("/api/sample/{id}", (string id, bool boolean, Input input) =>
{
    return TypedResults.Ok();
})
.WithOpenApi();

app.Run();

public record class Input(Guid id, int Number);

internal class ResetContentOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation.Parameters?.Any() ?? false)
        {
            foreach (var parameter in operation.Parameters)
            {
                if (parameter.In is ParameterLocation.Query or ParameterLocation.Path or ParameterLocation.Header)
                {
                    parameter.Content = null;
                }
            }
        }
    }
}