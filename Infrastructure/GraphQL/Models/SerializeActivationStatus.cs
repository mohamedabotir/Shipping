using System.Text.Json.Serialization;
using Common.Constants;

namespace Infrastructure.GraphQL.Models;

public class SerializeActivationStatus
{
    [JsonPropertyName("activationStatus")]
    public int ActivationStatus { get; set; }
}