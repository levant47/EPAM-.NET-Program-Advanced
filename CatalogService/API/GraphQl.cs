public class Query : ObjectGraphType
{
    public Query()
    {
        Field<IEnumerable<ItemEntity>>("item")
            .Argument<int?>("categoryId", nullable: true)
            .Argument<int?>("page", nullable: true)
            .Argument<int?>("pageSize", nullable: true)
            .ResolveAsync(async context =>
            {
                var categoryId = context.GetArgument<int?>("categoryId");
                var page = context.GetArgument<int?>("page") ?? 1;
                var pageSize = context.GetArgument<int?>("pageSize") ?? 10;
                return (await context.RequestServices!.GetRequiredService<IItemService>().GetByFilter(new()
                {
                    CategoryId = categoryId,
                    Page = page,
                    PageSize = pageSize,
                })).Items;
            });

        Field<IEnumerable<CategoryEntity>>("category")
            .ResolveAsync(async context => await context.RequestServices!.GetRequiredService<ICategoryService>().GetAll());
    }
}

public class Mutation : ObjectGraphType
{
    public Mutation()
    {
        Field<CategoryEntity>("createCategory")
            .Argument<CategoryCreateMutationInput>("category")
            .ResolveAsync(async context =>
                await context.RequestServices!.GetRequiredService<ICategoryService>().Create(context.GetArgument<CategoryCreateDto>("category")));

        Field<CategoryEntity>("updateCategory")
            .Argument<int>("id")
            .Argument<CategoryUpdateMutationInput>("category")
            .ResolveAsync(async context => await context.RequestServices!.GetRequiredService<ICategoryService>().Update(
                context.GetArgument<int>("id"),
                context.GetArgument<CategoryUpdateDto>("category")
            ));

        Field<string>("deleteCategory")
            .Argument<int>("id")
            .ResolveAsync(async context =>
            {
                await context.RequestServices!.GetRequiredService<ICategoryService>().Delete(context.GetArgument<int>("id"));
                return "deleted";
            });

        Field<ItemEntity>("createItem")
            .Argument<ItemCreateMutationInput>("item")
            .ResolveAsync(async context =>
                await context.RequestServices!.GetRequiredService<IItemService>().Create(context.GetArgument<ItemCreateDto>("item")));

        Field<ItemEntity>("updateItem")
            .Argument<int>("id")
            .Argument<ItemUpdateMutationInput>("item")
            .ResolveAsync(async context => await context.RequestServices!.GetRequiredService<IItemService>().Update(
                context.GetArgument<int>("id"),
                context.GetArgument<ItemUpdateDto>("item")
            ));

        Field<string>("deleteItem")
            .Argument<int>("id")
            .ResolveAsync(async context =>
            {
                await context.RequestServices!.GetRequiredService<IItemService>().Delete(context.GetArgument<int>("id"));
                return "deleted";
            });
    }
}

// result types
public class GenericGraphType<T> : ObjectGraphType<T> { public GenericGraphType() => GraphQlHelper.GenerateGraphFields(this); }
public class ItemQuery : GenericGraphType<ItemEntity> { }
public class CategoryQuery : GenericGraphType<CategoryEntity> { }

// input types
public class GenericMutationInput<T> : InputObjectGraphType<T> { public GenericMutationInput() => GraphQlHelper.GenerateGraphFields(this); }
public class CategoryCreateMutationInput : GenericMutationInput<CategoryCreateDto> { }
public class CategoryUpdateMutationInput : GenericMutationInput<CategoryUpdateDto> { }
public class ItemCreateMutationInput : GenericMutationInput<ItemCreateDto> { }
public class ItemUpdateMutationInput : GenericMutationInput<ItemUpdateDto> { }


public static class GraphQlHelper
{
    public static void GenerateGraphFields<T>(ComplexGraphType<T> graph)
    {
        Expression<Action<ComplexGraphType<T>>> expression = g => g.Field<string>("test", false);
        var fieldMethod = ((MethodCallExpression)expression.Body).Method.GetGenericMethodDefinition();
        foreach (var property in typeof(T).GetProperties())
        {
            fieldMethod.MakeGenericMethod(property.PropertyType)
                .Invoke(graph, new object?[] { property.Name, Nullable.GetUnderlyingType(property.PropertyType) != null });
        }
    }
}
