﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Placeholders.API.Interfaces
{
    public interface IPlaceholderPlayerHook
    {
        string OnRequest(string uuid, string identifier);
    }
}
