using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MABMoney.Domain
{
    public interface IAuditable
    {
        int CreatedBy { get; set; }
        DateTime CreatedDate { get; set; }
        int LastModifiedBy { get; set; }
        DateTime LastModifiedDate { get; set; }
    }
}
