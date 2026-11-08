using Claims.Auditing.Models;

namespace Claims.Auditing
{
    public interface IAuditer
    {
        Task AuditAsync(AuditEvent auditEvent);

    }
}