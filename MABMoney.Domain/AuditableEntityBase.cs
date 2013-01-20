using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MABMoney.Domain
{
    public abstract class AuditableEntityBase
    {
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int LastModifiedBy { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public bool Deleted { get; set; }
        public int DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
    }
}
