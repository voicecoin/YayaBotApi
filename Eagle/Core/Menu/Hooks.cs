﻿using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Interfaces;

namespace Core.Menu
{
    public class Hooks : IHookDbInitializer
    {
        public int Priority => 1;

        public void Load(IHostingEnvironment env, CoreDbContext dc)
        {
            
        }
    }
}
