using API.Models.Response;
using Microsoft.AspNetCore.Mvc;

namespace API.Infrastructure.Extensions
{
    public static class HateoasExtensions
    {
        public static void AddStandardLinks(
            this LinkedResource resource,
            HttpContext httpContext,
            IUrlHelper url,
            string id,
            string getRouteName,
            string? deleteRouteName = null,
            string? updateRouteName = null)
        {
            resource.Links.Clear();

            var version = httpContext.GetRequestedApiVersion()?.ToString() ?? "1.0";

            // Self Link
            // Since 'id' is the HashId string (e.g., "jR8vW2"), 
            // the URL generated will be/api/v1/authors/jR8vW2
            resource.Links.Add(new LinkDto(
                Href: url.Link(getRouteName, new { id, version }) ?? "",
                Rel: "self",
                Method: "GET"));

            // Delete Link
            if (!string.IsNullOrEmpty(deleteRouteName))
            {
                resource.Links.Add(new LinkDto(
                    Href: url.Link(deleteRouteName, new { id, version }) ?? "",
                    Rel: "delete_resource",
                    Method: "DELETE"));
            }

            // Update Link
            if (!string.IsNullOrEmpty(updateRouteName))
            {
                resource.Links.Add(new LinkDto(
                    Href: url.Link(updateRouteName, new { id, version }) ?? "",
                    Rel: "update_resource",
                    Method: "PUT"));
            }
        }
    }
}