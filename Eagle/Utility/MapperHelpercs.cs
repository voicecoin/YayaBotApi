﻿using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Utility
{
    public static class MapperHelpercs
    {
        public static T Map<T>(this Object source)
        {
            return Mapper.Map<T>(source);
        }
    }
}
