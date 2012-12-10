﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Forseti.TRX.Reporting
{
    public class Times : ICanGeneratetTrxPart
    {
        const string _elementName = "Times";

        public DateTime Creation { get { return StartTime; } }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public XElement GenerateTrxPart()
        {
            var element = new XElement(TrxBuilder.XMLNS + _elementName);
            element.SetAttributeValue("creation", Creation);
            element.SetAttributeValue("start", StartTime);
            element.SetAttributeValue("finish", EndTime);
            return element;
        }
    }
}