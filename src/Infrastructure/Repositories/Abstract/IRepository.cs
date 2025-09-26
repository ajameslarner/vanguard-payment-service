using System.Linq.Expressions;
using Infrastructure.Entities.Base;

namespace Infrastructure.Repositories.Abstract;

public interface IRepository<T> where T : TEntity
{
    public T FindOne(Expression<Func<T, bool>> filterExpression);
    public T InsertOne(T document, bool saveChanges = false);
    public T ReplaceOne(T document, bool saveChanges = false);
    public void ReplaceMany(T[] documents);
    public void Save();

    public Task<T> FindOneAsync(Expression<Func<T, bool>> filterExpression, CancellationToken cancellationToken = default);
    public Task<T> InsertOneAsync(T document, bool saveChanges = false, CancellationToken cancellationToken = default);
    public Task<T> ReplaceOneAsync(T document, bool saveChanges = false, CancellationToken cancellationToken = default);
    public Task ReplaceManyAsync(T[] documents, CancellationToken cancellationToken = default);
    public Task SaveAsync(CancellationToken cancellationToken);
}
