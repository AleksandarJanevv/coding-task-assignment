using Claims.Auditing.Models;
using MongoDB.Driver;

namespace Claims.Auditing
{
    /// <summary>
    /// Provides functionality for persisting audit events in MongoDB.
    /// </summary>
    /// <remarks>
    /// Stores Claim and Cover audit records in separate MongoDB collections.
    /// The target collection is determined by the entity type of the audit event.
    /// </remarks>
    public class Auditer : IAuditer
    {
        private readonly IMongoCollection<ClaimAudit> _claimAudits;
        private readonly IMongoCollection<CoverAudit> _coverAudits;

        /// <summary>
        /// Initializes the MongoDB collections used to store audit records.
        /// </summary>
        /// <param name="database">The MongoDB database instance.</param>
        public Auditer(IMongoDatabase database)
        {
            _claimAudits = database.GetCollection<ClaimAudit>("ClaimAudits");
            _coverAudits = database.GetCollection<CoverAudit>("CoverAudits");
        }

        /// <summary>
        /// Persists an audit event in the appropriate MongoDB collection.
        /// </summary>
        /// <param name="auditEvent">The audit event containing the entity type, ID, action and timestamp.</param>
        /// <returns>A task representing the asynchronous database operation.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when the audit event contains an unsupported entity type.
        /// </exception>
        public async Task AuditAsync(AuditEvent auditEvent)
        {
            switch (auditEvent.EntityType)
            {
                case AuditEntityType.Claim:
                    await _claimAudits.InsertOneAsync(new ClaimAudit
                    {
                        Created = auditEvent.Created,
                        HttpRequestType = auditEvent.Action,
                        ClaimId = auditEvent.EntityId
                    });
                    break;

                case AuditEntityType.Cover:
                    await _coverAudits.InsertOneAsync(new CoverAudit
                    {
                        Created = auditEvent.Created,
                        HttpRequestType = auditEvent.Action,
                        CoverId = auditEvent.EntityId
                    });
                    break;

                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(auditEvent.EntityType));
            }
        }
    }
}
