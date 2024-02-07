﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessadorAutonomo.Entities
{
    public class SubProcess
    {
        public int Id { get; set; }

        public TimeSpan Duration { get; set; }

        public bool isConcluded { get; set; } = false;

        public int ProcessId { get; set; }
        public Process Process { get; set; } = null!;

    }
}