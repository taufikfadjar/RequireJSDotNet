﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequireJSNetFrameworkCore.Configuration
{
    public interface IFileReader
    {
        string ReadFile(string path);
    }
}
