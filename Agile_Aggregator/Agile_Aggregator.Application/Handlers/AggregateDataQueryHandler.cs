using System.Diagnostics;
using Api_Aggregator.Application.Queries;
using MediatR;

public class AggregateDataQueryHandler
    : IRequestHandler<AggregateDataQuery, IEnumerable<AggregatedItemDto>>
{
    private readonly IEnumerable<IExternalApiClient> _clients;
    private readonly IStatisticsRepository _stats;

    public AggregateDataQueryHandler(
        IEnumerable<IExternalApiClient> clients,
        IStatisticsRepository stats)
    {
        _clients = clients;
        _stats = stats;
    }

    public async Task<IEnumerable<AggregatedItemDto>> Handle(
        AggregateDataQuery request, CancellationToken ct)
    {
        var tasks = _clients.Select(async c =>
        {
            var sw = Stopwatch.StartNew();
            var items = await c.FetchAsync(request.Dto);
            sw.Stop();
            _stats.Record(c.GetType().Name, sw.Elapsed);
            return items;
        });

        var results = await Task.WhenAll(tasks);
        var flat = results.SelectMany(x => x);

        // apply filtering/sorting here if needed...
        return flat
            .OrderByDescending(i => i.PublishedAt)
            .Select(i => new AggregatedItemDto
            {
                Title = i.Title,
                Source = i.Source.ToString(),
                PublishedAt = i.PublishedAt,
                Url = i.Url
            });
    }
}
