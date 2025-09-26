using System.Linq.Expressions;
using Infrastructure.Entities.Base;
using Infrastructure.Extensions;
using Infrastructure.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositories;

public class GenericRepository<T> : IRepository<T> where T : TEntity
{
    protected readonly ILogger<GenericRepository<T>> _logger;
    protected readonly DbContext _context;

    public GenericRepository(ILogger<GenericRepository<T>> logger, DbContext context)
    {
        _logger = logger;
        _context = context;

        _context.Database.EnsureCreated();
    }

    public T FindOne(Expression<Func<T, bool>> filterExpression)
    {
        try
        {
            return _context.Set<T>()
                           .TagWithCallSite()
                           .AsNoTrackingWithIdentityResolution()
                           .FirstOrDefault(filterExpression);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{ex?.InnerException?.GetType().Name ?? ex.GetType().Name}: {ex?.InnerException?.Message ?? ex.Message}");
            throw;
        }
    }

    public virtual async Task<T> FindOneAsync(Expression<Func<T, bool>> filterExpression, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Set<T>()
                                 .TagWithCallSite()
                                 .AsNoTrackingWithIdentityResolution()
                                 .FirstOrDefaultAsync(filterExpression, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{ex?.InnerException?.GetType().Name ?? ex.GetType().Name}: {ex?.InnerException?.Message ?? ex.Message}");
            throw;
        }
    }

    public T InsertOne(T document, bool saveChanges = false)
    {
        document.InitEntity();

        _context.Add(document);

        if (saveChanges)
            Save();

        return document;
    }

    public virtual async Task<T> InsertOneAsync(T document, bool saveChanges = false, CancellationToken cancellationToken = default)
    {
        try
        {
            document.InitEntity();

            await _context.AddAsync(document, cancellationToken);

            if (saveChanges)
                await SaveAsync(cancellationToken);

            return document;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{ex?.InnerException?.GetType().Name ?? ex.GetType().Name}: {ex?.InnerException?.Message ?? ex.Message}");
            throw;
        }
    }

    public T ReplaceOne(T document, bool saveChanges = false)
    {
        document.SetEntity();

        var result = _context.Update(document).Entity;

        if (saveChanges)
            Save();

        return result;
    }

    public virtual async Task<T> ReplaceOneAsync(T document, bool saveChanges = false, CancellationToken cancellationToken = default)
    {
        try
        {
            document.SetEntity();

            var result = _context.Update(document).Entity;

            if (saveChanges)
                await SaveAsync(cancellationToken);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{ex?.InnerException?.GetType().Name ?? ex.GetType().Name}:{ex?.InnerException?.Message ?? ex.Message}");
            throw;
        }
    }

    public virtual void ReplaceMany(T[] documents)
    {
        try
        {
            documents.SetEntities();

            _context.UpdateRange(documents);
            Save();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{ex?.InnerException?.GetType().Name ?? ex.GetType().Name}:{ex?.InnerException?.Message ?? ex.Message}");
            throw;
        }
    }

    public virtual async Task ReplaceManyAsync(T[] documents, CancellationToken cancellationToken = default)
    {
        try
        {
            documents.SetEntities();

            _context.UpdateRange(documents);
            await SaveAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{ex?.InnerException?.GetType().Name ?? ex.GetType().Name}:{ex?.InnerException?.Message ?? ex.Message}");
            throw;
        }
    }

    public async Task SaveAsync(CancellationToken cancellationToken)
    {
        var result = await _context.SaveChangesAsync(cancellationToken);

        if (result > -1)
            _logger.LogInformation($"Successfully modified {result} entries!");
        else
            _logger.LogWarning("No entries modified!");
    }

    public void Save()
    {
        var result = _context.SaveChanges();

        if (result > -1)
            _logger.LogInformation($"Successfully modified {result} entries!");
        else
            _logger.LogWarning("No entries modified!");
    }
}
