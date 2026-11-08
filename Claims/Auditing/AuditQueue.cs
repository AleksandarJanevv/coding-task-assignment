using Claims.Auditing.Models;
using System.Threading.Channels;

namespace Claims.Auditing
{
    /// <summary>
    /// Provides a thread-safe, in-memory queue for audit events.
    /// </summary>
    /// <remarks>
    /// Uses a bounded Channel to temporarily store audit events until
    /// they are consumed by the background auditing service.
    /// </remarks>
    public class AuditQueue : IAuditQueue
    {

        private readonly Channel<AuditEvent> _queue;

        /// <summary>
        /// Initializes the audit queue with a maximum capacity of 1000 events.
        /// </summary>
        public AuditQueue()
        {
            _queue = Channel.CreateBounded<AuditEvent>(
                new BoundedChannelOptions(1000)
                {
                    // Wait when the queue is full instead of discarding events.
                    // This provides backpressure to producers.
                    FullMode = BoundedChannelFullMode.Wait
                });
        }

        /// <summary>
        /// Adds an audit event to the queue.
        /// </summary>
        /// <param name="auditEvent">The audit event to enqueue.</param>
        /// <returns>A task representing the asynchronous enqueue operation.</returns>
        public ValueTask EnqueueAsync(AuditEvent auditEvent)
        {
            return _queue.Writer.WriteAsync(auditEvent);
        }

        /// <summary>
        /// Reads audit events from the queue as they become available.
        /// </summary>
        /// <param name="cancellationToken">
        /// Token used to cancel the reading operation.
        /// </param>
        /// <returns>An asynchronous stream of audit events.</returns>
        public IAsyncEnumerable<AuditEvent> ReadAllAsync(CancellationToken cancellationToken)
        {
            return _queue.Reader.ReadAllAsync(cancellationToken);
        }
    }
    
 }
