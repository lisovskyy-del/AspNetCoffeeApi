namespace AspNetCoffeeApi;

public record OrderRequest(string? Drink);

public class CoffeeMachineException(string message, string reason) : Exception(message)
{
    public string Reason { get; } = reason;
}