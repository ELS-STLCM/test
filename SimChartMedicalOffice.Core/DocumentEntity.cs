﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimChartMedicalOffice.Core
{
    public abstract class DocumentEntity : AbstractEntity
    {
        private string _guidValue;
        public string UniqueIdentifier { get { return _guidValue; } set { _guidValue = value; } }
        public string Url { get; set; }
        public void SetGuidValue()
        {
            this._guidValue = Guid.NewGuid().ToString();
        }
        public string GetNewGuidValue()
        {
            return Guid.NewGuid().ToString();
        }

        public string ParentReferenceGuid { get; set; }
    }
}
