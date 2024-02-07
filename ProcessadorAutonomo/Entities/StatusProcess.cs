using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessadorAutonomo.Entities
{
    public enum StatusProcess
    {
        Created,
        Scheduled,
        InProgress,
        Paused,
        Canceled,
        Completed
    }
}
