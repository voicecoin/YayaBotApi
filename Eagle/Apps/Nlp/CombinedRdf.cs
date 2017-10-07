﻿using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utility;

namespace Apps.Nlp
{
    /// <summary>
    /// 从多个数据源查询出三元组信息，并组合返回
    /// </summary>
    public class CombinedRdf
    {
        public static List<Triple> QueryEntity(CoreDbContext Dc, string s)
        {
            List<Triple> combined = new List<Triple>();

            try
            {
                List<Triple> triple1 = WikiInfoBoxRdf.QueryEntity(Dc, s);
                combined.AddRange(triple1);
            }
            catch (Exception ex)
            {
                ex.Message.Log(MyLogLevel.ERROR);
            }

            try
            {
                List<Triple> triple2 = CnDbPeidaRdf.QueryEntity(Dc, s);
                combined.AddRange(triple2);
            }
            catch (Exception ex)
            {
                ex.Message.Log(MyLogLevel.ERROR);
            }

            return combined.Distinct(x => x.Predicate).ToList();
        }
    }
}