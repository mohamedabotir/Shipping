namespace Infrastructure.GraphQL.Models;

public class GraphQLResponse<T>
{
    public T Data { get; set; }
}