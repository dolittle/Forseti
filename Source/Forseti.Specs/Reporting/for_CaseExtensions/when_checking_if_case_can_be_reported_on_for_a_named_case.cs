﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Forseti.Reporting;
using Machine.Specifications;
using Forseti.Suites;

namespace Forseti.Specs.Reporting.for_CaseExtensions
{
    [Subject(typeof(CaseExtensions))]
    public class when_checking_if_case_can_be_reported_on_for_a_named_case
    {

        static Case @case;
        static bool can_be_reported_on;

        Establish context = () => @case = new Case { Name  = "I am a naned case that can be run!" };

        Because of = () => can_be_reported_on = @case.CanBeReportedOn();

        It should_not_be_able_to_report = () => can_be_reported_on.ShouldBeTrue();
    }
}