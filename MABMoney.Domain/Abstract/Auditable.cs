using System;

namespace MABMoney.Domain.Abstract
{
    public abstract class Auditable
    {
        public int CreatedBy { get; private set; }
        public DateTime CreatedDate { get; private set; }
        public int LastModifiedBy { get; private set; }
        public DateTime LastModifiedDate { get; private set; }
        public bool Deleted { get; private set; }
        public int? DeletedBy { get; private set; }
        public DateTime? DeletedDate { get; private set; }
    }
}
