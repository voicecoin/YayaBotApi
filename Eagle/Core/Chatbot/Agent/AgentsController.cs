﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Core;
using Apps.Chatbot.DomainModels;
using Core.DbTables;
using Utility;

namespace Apps.Chatbot.Agent
{
    public class AgentsController : CoreController
    {
        // GET: v1/Agents
        [HttpGet("{userId}/Query")]
        public DmPageResult<DmAgent> GetAgents([FromRoute] string userId, string name, [FromQuery] int page = 1)
        {
            var query = dc.Chatbot_Agents.Where(x => x.UserId == userId);
            if (!String.IsNullOrEmpty(name))
            {
                query = query.Where(x => x.Name.Contains(name));
            }

            var total = query.Count();

            int size = 20;

            var items = query.Skip((page - 1) * size).Take(size).Select(x => x.Map<DmAgent>()).ToList();
            return new DmPageResult<DmAgent> { Total = total, Page = page, Size = size, Items = items };
        }

        // GET: v1/Agents/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAgent([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var agents = await dc.Chatbot_Agents.SingleOrDefaultAsync(m => m.Id == id);

            if (agents == null)
            {
                return NotFound();
            }

            var agent = agents.Map<DmAgent>();

            return Ok(agent);
        }

        // PUT: v1/Agents/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAgents([FromRoute] string id, [FromBody] DmAgent agentModel)
        {
            if (id != agentModel.Id)
            {
                return BadRequest("Agent id not match.");
            }

            var agentRecord = dc.Chatbot_Agents.Find(id);

            agentRecord.Name = agentModel.Name;
            agentRecord.Description = agentModel.Description;
            agentRecord.Language = agentModel.Language;
            agentRecord.Avatar = agentModel.Avatar;

            dc.Entry(agentRecord).State = EntityState.Modified;

            try
            {
                await dc.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AgentsExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: v1/Agents
        [HttpPost]
        public async Task<IActionResult> PostAgent([FromBody] DmAgent agentModel)
        {
            Agents agentRecord = agentModel.Map<Agents>();
            agentRecord.Language = "zh-cn";
            agentRecord.ClientAccessToken = Guid.NewGuid().ToString("N");
            agentRecord.DeveloperAccessToken = Guid.NewGuid().ToString("N");
            agentRecord.CreatedDate = DateTime.UtcNow;

            dc.Chatbot_Agents.Add(agentRecord);
            await dc.SaveChangesAsync();

            return CreatedAtAction("GetAgents", new { id = agentRecord.Id }, new { id = agentRecord.Id });
        }

        // DELETE: v1/Agents/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAgents([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var agents = await dc.Chatbot_Agents.SingleOrDefaultAsync(m => m.Id == id);
            if (agents == null)
            {
                return NotFound();
            }

            dc.Chatbot_Agents.Remove(agents);
            await dc.SaveChangesAsync();

            return Ok(agents);
        }

        private bool AgentsExists(string id)
        {
            return dc.Chatbot_Agents.Any(e => e.Id == id);
        }
    }
}