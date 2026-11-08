namespace Claims.Auditing
{
    /// <summary>
    /// Background service responsible for processing audit events
    /// from the audit queue and persisting them asynchronously.
    /// </summary>
    /// <remarks>
    /// The service continuously reads audit events from the queue
    /// and delegates their persistence to the IAuditer implementation.
    ///
    /// Each event is processed independently and retried up to
    /// three times if an error occurs. Failed attempts use an
    /// increasing delay before the next retry.
    /// </remarks>
    public class AuditBackgroundService : BackgroundService
    {
        private readonly IAuditQueue _auditQueue;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<AuditBackgroundService> _logger;

        public AuditBackgroundService(IAuditQueue auditQueue, IServiceScopeFactory scopeFactory, ILogger<AuditBackgroundService> logger)
        {
            _auditQueue = auditQueue;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        /// <summary>
        /// Continuously reading and processing audit events until
        /// the application shuts down.
        /// </summary>
        /// <param name="stoppingToken">
        /// Token used to signal cancellation during application shutdown.
        /// </param>
        /// <returns>
        /// A task representing the background processing operation.
        /// </returns>
        /// <remarks>
        /// Each audit event is processed with a maximum of three attempts.
        /// If an attempt fails, the service logs a warning and waits
        /// before retrying. The delay increases with each attempt.
        ///
        /// If all attempts fail, an error is logged and processing
        /// continues with the next queued event.
        /// </remarks>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (var auditEvent in _auditQueue.ReadAllAsync(stoppingToken))
            {
                var successful = false;

                for (var attempt = 1; attempt <= 3; attempt++)
                {
                    try
                    {
                        // Create a new scope for each audit event because IAuditer is registered as a scoped service and BackgroundService is a singleton.
                        using var scope = _scopeFactory.CreateScope();

                        var auditer = scope.ServiceProvider.GetRequiredService<IAuditer>();

                        await auditer.AuditAsync(auditEvent);

                        successful = true;
                        break;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(
                            ex,
                            "Audit failed. Attempt {Attempt}/3. Entity: {EntityId}",
                            attempt,
                            auditEvent.EntityId);

                        // Increase the delay between retry attempts.
                        if (attempt < 3)
                        {
                            await Task.Delay(TimeSpan.FromSeconds(attempt * 2), stoppingToken);
                        }
                    }
                }

                if (!successful)
                {
                    _logger.LogError("Audit event failed after 3 attempts. Entity: {EntityId}", auditEvent.EntityId);
                    // Addition could be made to insert the log in SQL Server table called AuditFailedLogs
                    // to have track of failed logs in database and not only in console.
                }
            }
        }
    }
}