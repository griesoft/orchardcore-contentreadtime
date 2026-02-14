using GraphQL.Types;
using Griesoft.OrchardCore.ContentReadTime.Models;

namespace Griesoft.OrchardCore.ContentReadTime.GraphQL;

/// <summary>
/// GraphQL object type for exposing <see cref="ContentReadTimePart"/> in GraphQL queries.
/// </summary>
public class ContentReadTimePartQueryObjectType : ObjectGraphType<ContentReadTimePart>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ContentReadTimePartQueryObjectType"/> class.
    /// </summary>
    public ContentReadTimePartQueryObjectType()
    {
        Name = "ContentReadTime";

        Field(x => x.Minutes)
            .Description("The estimated reading time in minutes for the content item.");
    }
}
