﻿using Apps.Chatbot.DomainModels;
using Apps.Chatbot.Intent;
using Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utility;

namespace Apps.Chatbot.DmServices
{
    public static partial class DmIntentService
    {
        public static void Load(this DomainModel<IntentEntity> intentModel)
        {
            CoreDbContext dc = intentModel.Dc;
            intentModel.LoadEntity();

            var intentExpressions = dc.Table<IntentExpressionEntity>().Where(x => x.IntentId == intentModel.Entity.Id).ToList();
            intentExpressions.ForEach(x =>
            {
                if (String.IsNullOrEmpty(x.DataJson))
                {
                    x.Data = new List<DmIntentExpressionItem>();
                } else
                {
                    x.Data = JsonConvert.DeserializeObject<List<DmIntentExpressionItem>>(x.DataJson);
                }
            });

            if (String.IsNullOrEmpty(intentModel.Entity.ContextsJson))
            {
                intentModel.Entity.Contexts = new List<string>();
            } else
            {
                intentModel.Entity.Contexts = JsonConvert.DeserializeObject<List<string>>(intentModel.Entity.ContextsJson);
            }

            intentModel.Entity.UserSays = intentExpressions.Select(expression => new IntentExpressionEntity
            {
                Id = expression.Id,
                Text = expression.Text,
                IntentId = expression.IntentId,
                Data = expression.Data.ToList()
            }).ToList();

            intentModel.Entity.Templates = intentModel.Entity.UserSays.Select(x => x.Data.GetTemplateString()).ToList();

            intentModel.Entity.Responses = dc.Table<IntentResponseEntity>()
                .Where(x => x.IntentId == intentModel.Entity.Id)
                .ToList();

            intentModel.Entity.Responses.ForEach(response =>
            {
                if (String.IsNullOrEmpty(response.AffectedContextsJson))
                {
                    response.AffectedContexts = new List<DmIntentResponseContext>();
                }
                else
                {
                    response.AffectedContexts = JsonConvert.DeserializeObject<List<DmIntentResponseContext>>(response.AffectedContextsJson);
                }

                // Load message
                response.Messages = dc.Table<IntentResponseMessageEntity>().Where(x => x.IntentResponseId == response.Id)
                    .Select(x => x.Map<IntentResponseMessageEntity>()).ToList();

                response.Messages.ForEach(message =>
                {
                    if (String.IsNullOrEmpty(message.SpeechesJson))
                    {
                        message.Speeches = new List<string>();
                    }
                    else
                    {
                        message.Speeches = JsonConvert.DeserializeObject<List<string>>(message.SpeechesJson);
                    }
                });


                // Load parameters
                response.Parameters = dc.Table<IntentResponseParameterEntity>().Where(x => x.IntentResponseId == response.Id)
                                    .Select(x => x.Map<IntentResponseParameterEntity>()).ToList();

                response.Parameters.ForEach(parameter => {
                    if (String.IsNullOrEmpty(parameter.PromptsJson))
                    {
                        parameter.Prompts = new List<string>();
                    }
                    else
                    {
                        parameter.Prompts = JsonConvert.DeserializeObject<List<string>>(parameter.PromptsJson);
                    }
                });
            });
        }


        public static void Add(this DomainModel<IntentEntity> intentModel)
        {
            if (!intentModel.AddEntity()) return;

            if(intentModel.Entity.Contexts != null)
            {
                intentModel.Entity.ContextsJson = JsonConvert.SerializeObject(intentModel.Entity.Contexts);
            }
            
            if(intentModel.Entity.Events != null)
            {
                intentModel.Entity.EventsJson = JsonConvert.SerializeObject(intentModel.Entity.Events);
            }

            intentModel.Entity.UserSays.ForEach(userSay =>
            {
                userSay.IntentId = intentModel.Entity.Id;
                var dm = new DomainModel<IntentExpressionEntity>(intentModel.Dc, userSay);
                dm.Add();
            });

            intentModel.Entity.Responses.ForEach(response =>
            {
                response.IntentId = intentModel.Entity.Id;
                var dm = new DomainModel<IntentResponseEntity>(intentModel.Dc, response);
                dm.Add();
            });
        }

        public static void Update(this DomainModel<IntentEntity> intentModel)
        {
            CoreDbContext dc = intentModel.Dc;
            var intentRecord = dc.Table<IntentEntity>().Find(intentModel.Entity.Id);
            intentRecord.Name = intentModel.Entity.Name;
            intentRecord.ModifiedDate = DateTime.UtcNow;

            // Remove all related data then create with same IntentId
            intentModel.Entity.UserSays.ForEach(expression => new DomainModel<IntentExpressionEntity>(dc, expression).Update());
            intentModel.Entity.Responses.ForEach(response => new DomainModel<IntentResponseEntity>(dc, response).Update());
        }
    }
}