
using System.Text.Json;
using Common.Constants;
using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using PurchaseOrderActivationStatus = Infrastructure.GraphQL.Models.PurchaseOrderActivationStatus;

namespace Infrastructure.GraphQL;

public class GraphQlClient
{
    private readonly GraphQLHttpClient graphQlHttpClient;
    public GraphQlClient(IOptions<PurchaseOrderGraphQLEndpoint> options)
    {
        graphQlHttpClient= new GraphQLHttpClient(options.Value.EndPoint, new NewtonsoftJsonSerializer());
    }

    public async Task<ActivationStatus> GetActivationStatus(string purchaseOrderNumber)
    {
        var query = """
                      query GetOrder($poNumber: String!) {
                      purchaseOrderByPurchaseOrderNumber(purchaseOrderNumber:$poNumber){
                        activationStatus
                      }
                    }
                    """;
        var request = new GraphQLRequest()
        {
            Query = query,
            Variables = new { poNumber = purchaseOrderNumber }
        };
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var response = await graphQlHttpClient.SendQueryAsync<PurchaseOrderActivationStatus>(request);
      Console.WriteLine(response.Data.PurchaseOrderByPurchaseOrderNumber.ActivationStatus);
      return (ActivationStatus)response.Data.PurchaseOrderByPurchaseOrderNumber.ActivationStatus;
    }
}