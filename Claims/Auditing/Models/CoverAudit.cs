using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Claims.Auditing.Models
{
    /// <summary>
    /// Represents an audit record for Covers stored in MongoDB.
    /// </summary>
    public class CoverAudit
    {
        /// <summary>
        /// Gets or sets the unique MongoDB document identifier.
        /// </summary>
        [BsonId]
        public ObjectId Id { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the Cover associated with this audit record.
        /// </summary>
        public string? CoverId { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the audited operation occurred.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the HTTP operation associated with the audit record,
        /// such as POST or DELETE.
        /// </summary>
        public string? HttpRequestType { get; set; }
    }
}
