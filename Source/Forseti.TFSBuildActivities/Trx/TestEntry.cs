﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Forseti.TFSBuildActivities.Trx
{
    public class TestEntry
    {
        public Guid TestId { get; set; }
        public Guid ExecutionId { get; set; }
        public Guid TestListId { get { return TestLists.RESULTSNOTINLIST; } }
    }
}
