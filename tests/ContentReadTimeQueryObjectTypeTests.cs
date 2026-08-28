using Griesoft.OrchardCore.ContentReadTime.GraphQL;
using Griesoft.OrchardCore.ContentReadTime.Models;
using GraphQL;
using GraphQL.Execution;
using GraphQL.Types;
using Xunit;

namespace Griesoft.OrchardCore.ContentReadTime.Tests;

/// <summary>
/// Tests for <see cref="ContentReadTimePartQueryObjectType"/> GraphQL type.
/// </summary>
public class ContentReadTimeQueryObjectTypeTests
{
    [Fact]
    public void Constructor_SetsCorrectName()
    {
        // Arrange & Act
        var objectType = new ContentReadTimePartQueryObjectType();

        // Assert
        Assert.Equal("ContentReadTime", objectType.Name);
    }

    [Fact]
    public void Constructor_RegistersMinutesField()
    {
        // Arrange & Act
        var objectType = new ContentReadTimePartQueryObjectType();

        // Assert
        var field = objectType.GetField("Minutes");
        Assert.NotNull(field);
    }

    [Fact]
    public void MinutesField_HasDescription()
    {
        // Arrange & Act
        var objectType = new ContentReadTimePartQueryObjectType();

        // Assert
        var field = objectType.GetField("Minutes");
        Assert.NotNull(field);
        Assert.NotNull(field.Description);
        Assert.NotEmpty(field.Description);
        Assert.Equal("The estimated reading time in minutes for the content item.", field.Description);
    }

    [Fact]
    public void GraphQLType_InheritsFromObjectGraphType()
    {
        // Arrange & Act
        var objectType = new ContentReadTimePartQueryObjectType();

        // Assert
        Assert.IsAssignableFrom<ObjectGraphType<ContentReadTimePart>>(objectType);
    }

    [Fact]
    public void GraphQLType_RegistersOnlyExpectedFields()
    {
        // Arrange & Act
        var objectType = new ContentReadTimePartQueryObjectType();

        // Assert
        // Should only have Minutes field (Fields property includes built-in fields like __typename)
        var customFields = objectType.Fields.Where(f => !f.Name.StartsWith("__")).ToList();
        Assert.Single(customFields);
        Assert.Equal("Minutes", customFields[0].Name);
    }

    [Fact]
    public void GraphQLType_CanBeInstantiated()
    {
        // Arrange & Act
        var objectType = new ContentReadTimePartQueryObjectType();

        // Assert
        Assert.NotNull(objectType);
        Assert.NotNull(objectType.Name);
        Assert.NotEmpty(objectType.Fields);
    }

    [Fact]
    public async Task ExecutedQuery_ResolvesMinutesValue()
    {
        // Arrange
        var schema = new Schema { Query = new TestQuery() };

        // Act
        var result = await new DocumentExecuter().ExecuteAsync(options =>
        {
            options.Schema = schema;
            options.Query = "{ contentReadTime { minutes } }";
        });

        // Assert
        Assert.Null(result.Errors);
        var data = Assert.IsAssignableFrom<ObjectExecutionNode>(result.Data).ToValue() as IDictionary<string, object?>;
        Assert.NotNull(data);
        var part = Assert.IsAssignableFrom<IDictionary<string, object?>>(data["contentReadTime"]);
        Assert.Equal(3, part["minutes"]);
    }

    private sealed class TestQuery : ObjectGraphType
    {
        public TestQuery()
        {
            Field<ContentReadTimePartQueryObjectType>("contentReadTime")
                .Resolve(_ => new ContentReadTimePart { Minutes = 3 });
        }
    }

    [Fact]
    public void MinutesField_HasCorrectFieldType()
    {
        // Arrange & Act
        var objectType = new ContentReadTimePartQueryObjectType();
        var field = objectType.GetField("Minutes");

        // Assert
        Assert.NotNull(field);
        Assert.Equal("Minutes", field.Name);
        Assert.Equal(typeof(NonNullGraphType<GraphQLClrOutputTypeReference<int>>), field.Type);
    }
}
