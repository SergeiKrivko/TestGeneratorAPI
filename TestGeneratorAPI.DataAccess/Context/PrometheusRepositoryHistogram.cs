using System.Diagnostics;
using System.Runtime.CompilerServices;
using Prometheus;

namespace TestGeneratorAPI.DataAccess.Context;

public class PrometheusRepositoryHistogram
{
    private static readonly Histogram Histogram = Metrics
        .CreateHistogram("repository_db_query_duration", "Duration of database queries",
            new HistogramConfiguration
            {
                Buckets = Histogram.ExponentialBuckets(0.01, 2, 10), // Настройте под ваши нужды
                LabelNames = new[] { "repository", "method" }
            });

    public static async Task<T> Measure<T>(Task<T> task, [CallerFilePath] string callerFile = "",
        [CallerMemberName] string callerMember = "")
    {
        var stopwatch = Stopwatch.StartNew();
        var res = await task;
        stopwatch.Stop();

        Histogram
            .WithLabels(Path.GetFileNameWithoutExtension(callerFile), callerMember)
            .Observe(stopwatch.Elapsed.TotalSeconds);

        return res;
    }

    public static async ValueTask<T> Measure<T>(ValueTask<T> task, [CallerFilePath] string callerFile = "",
        [CallerMemberName] string callerMember = "")
    {
        var stopwatch = Stopwatch.StartNew();
        var res = await task;
        stopwatch.Stop();

        Histogram
            .WithLabels(Path.GetFileNameWithoutExtension(callerFile), callerMember)
            .Observe(stopwatch.Elapsed.TotalSeconds);

        return res;
    }

    public static async Task Measure(Task task, [CallerFilePath] string callerFile = "",
        [CallerMemberName] string callerMember = "")
    {
        var stopwatch = Stopwatch.StartNew();
        await task;
        stopwatch.Stop();

        Histogram
            .WithLabels(Path.GetFileNameWithoutExtension(callerFile), callerMember)
            .Observe(stopwatch.Elapsed.TotalSeconds);
    }
}