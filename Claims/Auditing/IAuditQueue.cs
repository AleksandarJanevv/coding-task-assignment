using Claims.Auditing.Models;

namespace Claims.Auditing
{
    public interface IAuditQueue
    {
        ValueTask EnqueueAsync(AuditEvent auditEvent);

        IAsyncEnumerable<AuditEvent> ReadAllAsync(CancellationToken cancellationToken);
    }
}
