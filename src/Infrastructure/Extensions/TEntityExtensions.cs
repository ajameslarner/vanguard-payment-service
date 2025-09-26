using Infrastructure.Entities.Base;

namespace Infrastructure.Extensions;

public static class TEntityExtensions
{
    public static TEntity InitEntity(this TEntity doc)
    {
        doc.Id = Guid.NewGuid();
        doc.CreatedAt = DateTimeOffset.UtcNow;

        return doc;
    }

    public static IEnumerable<TEntity> InitEntities(this IEnumerable<TEntity> docs)
    {
        foreach (var doc in docs)
        {
            doc.Id = Guid.NewGuid();
            doc.CreatedAt = DateTimeOffset.UtcNow;
        }

        return docs;
    }

    public static TEntity SetEntity(this TEntity doc)
    {
        doc.LastUpdatedBy ??= "SYSTEM";
        doc.LastUpdatedOn = DateTimeOffset.UtcNow;

        return doc;
    }

    public static IEnumerable<TEntity> SetEntities(this IEnumerable<TEntity> docs)
    {
        foreach (var doc in docs)
        {
            doc.LastUpdatedBy ??= "SYSTEM";
            doc.LastUpdatedOn = DateTimeOffset.UtcNow;
        }
        return docs;
    }
}
