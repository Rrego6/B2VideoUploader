using Microsoft.Extensions.Logging;

public sealed class CustomLogger : ILogger
{
    private List<Action<string>> logSubscribers;
    
    //TODO: use delegates/events
    //TODO: dont use singleton for logging

    public CustomLogger()
    {
        logSubscribers = new List<Action<string>>();
    }

    public IDisposable BeginScope<TState>(TState state) => default!;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }
        var outputString = $"[{eventId.Id,2}: {logLevel,-12}]: {formatter(state, exception)}";
        Console.WriteLine(outputString);
        foreach (var subscriber in logSubscribers)
        {
            subscriber(outputString);
        }
    }

    /**
     * add subscriber to handle log output
     */
    public void addLoggingSubscriber(Action<string> logSubcriber)
    {
        logSubscribers.Add(logSubcriber);
    }
}