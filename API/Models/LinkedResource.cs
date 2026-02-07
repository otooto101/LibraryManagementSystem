using System.Text.Json.Serialization;

namespace API.Models.Response
{
    public abstract class LinkedResource
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public List<LinkDto> Links { get; set; } = new();
    }

    public record LinkDto(string Href, string Rel, string Method);
}