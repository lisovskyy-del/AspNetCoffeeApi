using AspNetCoffeeApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Extensions["traceId"] = context.HttpContext.TraceIdentifier;

        if (context.Exception is TimeoutException)
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status504GatewayTimeout;
            context.ProblemDetails.Status = StatusCodes.Status504GatewayTimeout;
            context.ProblemDetails.Title = "Operation timed out";
            context.ProblemDetails.Detail = "The coffee machine took too long to respond.";
            context.ProblemDetails.Extensions["reason"] = "MACHINE_TIMEOUT";
        }
    };
});

builder.Services.AddExceptionHandler<CoffeeMachineExceptionHandler>();

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

app.MapPost("/order", (OrderRequest request) =>
{
    var drink = request.Drink!.ToLowerInvariant();

    return drink switch
    {
        "espresso" => throw new CoffeeMachineException("Water tank is empty.", "OUT_OF_WATER"),
        "cappuccino" => throw new TimeoutException("Milk frother did not respond in time."),
        _ => Results.Ok(new { message = $"Your {drink} is ready!" })
    };
})
.AddEndpointFilter<CoffeeValidationFilter>();

app.Run();