﻿using System.Collections.Generic;
using System;

namespace Forseti.Suites
{
    public class Case
    {
		public Case()
		{
			Result = new CaseResult();
		}
		
        List<Case> _children = new List<Case>();

        public Description Description { get; set; }
        public Case Parent { get; set; }
        public string Name { get; set; }

        public static Case DummyCase { get { return new Case { Name = "DUMMYCASE" }; } }
		
		public CaseResult Result { get; set; }

        public IEnumerable<Case> Children { get { return _children; } }

        public void AddChildCase(Case @case)
        {
            @case.Parent = this;
            @case.Description = this.Description;
            _children.Add(@case);
        }

        public static bool IsDummyOrEmptyCase(Case @case)
        {
            return @case == null || String.IsNullOrEmpty(@case.Name) || @case.Name == Case.DummyCase.Name;
        }
    }
}
