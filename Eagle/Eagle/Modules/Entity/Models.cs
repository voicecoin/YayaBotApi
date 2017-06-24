﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eagle.Models
{
    public class EntityModel
    {
        public String Id { get; set; }
        public String AgentId { get; set; }
        public String Name { get; set; }
        public Boolean IsEnum { get; set; }
        public Boolean IsOverridable { get; set; }
        public IEnumerable<EntityEntryModel> Entries { get; set; }
    }

    public class EntityEntryModel
    {
        public String Value { get; set; }
        public String Template { get; set; }
        public IEnumerable<String> Synonyms { get; set; }
    }
}
