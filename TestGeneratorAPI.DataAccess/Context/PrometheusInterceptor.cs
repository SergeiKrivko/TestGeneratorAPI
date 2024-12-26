using Microsoft.EntityFrameworkCore.Diagnostics;
using Prometheus;
using System.Data.Common;
using System.Diagnostics;

namespace TestGeneratorAPI.DataAccess.Context;

public class PrometheusInterceptor : DbCommandInterceptor
{
    private static readonly Histogram QueryDurationHistogram = Metrics
        .CreateHistogram("db_query_duration_seconds", "Duration of database queries",
            new HistogramConfiguration
            {
                Buckets = Histogram.ExponentialBuckets(0.01, 2, 10), // Настройте под ваши нужды
                LabelNames = new[] { "command_type" }
            });

    public override DbDataReader ReaderExecuted(DbCommand command, CommandExecutedEventData eventData,
        DbDataReader result)
    {
        MeasureExecutionTime(command, () => base.ReaderExecuted(command, eventData, result));
        return result;
    }

    public override int NonQueryExecuted(DbCommand command, CommandExecutedEventData eventData, int result)
    {
        MeasureExecutionTime(command, () => base.NonQueryExecuted(command, eventData, result));
        return result;
    }

    public override object ScalarExecuted(DbCommand command, CommandExecutedEventData eventData, object? result)
    {
        MeasureExecutionTime(command, () => base.ScalarExecuted(command, eventData, result));
        return result;
    }

    // Асинхронная версия для ReaderExecuted
    public override async ValueTask<DbDataReader> ReaderExecutedAsync(DbCommand command, CommandExecutedEventData eventData, DbDataReader result, CancellationToken cancellationToken = new())
    {
        await MeasureExecutionTimeAsync(command, cancellationToken);
        return await base.ReaderExecutedAsync(command, eventData, result, cancellationToken);
    }

    // Асинхронная версия для NonQueryExecuted
    public override async ValueTask<int> NonQueryExecutedAsync(DbCommand command, CommandExecutedEventData eventData, int result, CancellationToken cancellationToken = new())
    {
        await MeasureExecutionTimeAsync(command, cancellationToken);
        return await base.NonQueryExecutedAsync(command, eventData, result, cancellationToken);
    }
    
    // Асинхронная версия для ScalarExecuted
    public override async ValueTask<object?> ScalarExecutedAsync(DbCommand command, CommandExecutedEventData eventData, object? result, CancellationToken cancellationToken = new())
    {
        await MeasureExecutionTimeAsync(command, cancellationToken);
        return await base.ScalarExecutedAsync(command, eventData, result, cancellationToken);
    }

    private static async Task MeasureExecutionTimeAsync(DbCommand command, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        // Даем асинхронный запрос пройти
        await Task.Yield();  // Завершаем асинхронную операцию
        stopwatch.Stop();

        // Обрабатываем тайминг запроса
        QueryDurationHistogram
            .WithLabels(command.CommandType.ToString())
            .Observe(stopwatch.Elapsed.TotalSeconds);
    }

    private static void MeasureExecutionTime(DbCommand command, Action execute)
    {
        var stopwatch = Stopwatch.StartNew();
        execute();
        stopwatch.Stop();

        QueryDurationHistogram
            .WithLabels(command.CommandType.ToString())
            .Observe(stopwatch.Elapsed.TotalSeconds);
    }
}