using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Claims.Auditing.Models
{
    /// <summary>
    /// Represents an audit record for a Claim stored in MongoDB.
    /// </summary>
    public class ClaimAudit
    {
        /// <summary>
        /// Gets or sets the unique MongoDB document identifier.
        /// </summary>
        [BsonId]
        public ObjectId Id { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the Claim associated with this audit record.
        /// </summary>
        public string? ClaimId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date and time when the audited operation occurred.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the HTTP operation associated with the audit record,
        /// such as POST or DELETE.
        /// </summary>
        public string? HttpRequestType { get; set; } = string.Empty;
    }
}
