using System;
using System.Collections.Generic;
using System.Text;

namespace DoimanDlls.Common
{
    public abstract class AuditableEntity
    {
        public DateTime CreatedAtUtc { get; private set; }
        public DateTime? UpdatedAtUtc { get; private set; }

        protected void MarkCreated(DateTime utcNow)
        {
            CreatedAtUtc = utcNow;
        }

        protected void MarkUpdated(DateTime utcNow)
        {
            UpdatedAtUtc = utcNow;
        }
    }
}
