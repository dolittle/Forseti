﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Forseti.Output.MSTest.Transformation
{
    public interface ITransformToTrx
    {
        XElement TransformToTrx();
    }
}
