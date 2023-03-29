using System;
using System.Collections.Generic;
using System.Linq;

namespace FFFui
{
    public interface IHasCompareSource
    {
        CompareSourceViewModel CompareSourceViewModel { get; }
        public ViewModel MainViewModel { get; }
        public string FileName { get;  }
        void FireCompareChanged();
        void BeforeAddToCompare();
    }
}
