namespace AspNetCoffeeApi;

public class CoffeeValidationFilter : IEndpointFilter
{
    private static readonly HashSet<string> AllowedDrinks = new(StringComparer.OrdinalIgnoreCase)
    {
        "latte", "cappuccino", "espresso"
    };

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var request = context.Arguments.OfType<OrderRequest>().FirstOrDefault();

        if (request is null || string.IsNullOrWhiteSpace(request.Drink) || !AllowedDrinks.Contains(request.Drink))
        {
            var errors = new Dictionary<string, string[]>
            {
                ["drink"] = ["Allowed values are: latte, cappuccino, espresso."]
            };

            return Results.ValidationProblem(
                errors: errors,
                title: "Invalid drink type",
                detail: $"Drink '{request?.Drink}' is not supported by this machine.",
                extensions: new Dictionary<string, object?>
                {
                    ["reason"] = "INVALID_DRINK_TYPE"
                }
            );
        }

        return await next(context);
    }
}