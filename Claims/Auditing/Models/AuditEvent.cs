namespace Claims.Auditing.Models
{
    /// <summary>
    /// Represents an event describing an operation performed on an auditable entity.
    /// </summary>
    /// <param name="EntityType">The type of entity being audited (Claim or Cover).</param>
    /// <param name="EntityId">The unique identifier of the affected entity.</param>
    /// <param name="Action">The operation performed, such as POST, PUT or DELETE.</param>
    /// <param name="Created">The date and time when the operation occurred.</param>
    public record AuditEvent(AuditEntityType EntityType, string EntityId, string Action, DateTime Created);
}
