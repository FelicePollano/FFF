﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFFui.Utils
{
    internal class HgRepository : IRepository
    {
        public void GetHistory(string filename, Action<RepoHelper.HistoryEntry> collectEntry)
        {
            throw new NotImplementedException();
        }
    }
}
