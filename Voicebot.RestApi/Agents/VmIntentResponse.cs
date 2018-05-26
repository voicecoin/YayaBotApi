﻿using BotSharp.Core.Intents;
using System;
using System.Collections.Generic;
using System.Text;

namespace Voicebot.RestApi.Agents
{
    /// <summary>
    /// Intent response
    /// </summary>
    public class VmIntentResponse
    {
        public Boolean ResetContexts { get; set; }

        public List<VmIntentResponseContext> AffectedContexts { get; set; }

        public List<VmIntentResponseParameter> Parameters { get; set; }

        public List<VmIntentResponseMessage> Messages { get; set; }
    }
}