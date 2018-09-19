using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Inkton.Nest.Cloud;
using Inkton.Nester;
using Inkton.Nest.Model;
using Websock.Database;
using Websock.Model;

namespace Websock.Controllers
{
    [ApiVersion("1.0")]
    [Route("[controller]")]
    [ApiController]
    public class MonitorController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IMessageRepository _messageRepo;

        public MonitorController(
            ILogger<MonitorController> logger,
            IMessageRepository messageRepo)
        {
            _logger = logger;
            _messageRepo = messageRepo;
        }

        // GET /Monitor/Sessions
        [HttpGet("Sessions")]
        public IActionResult GetSessions()
        {
            try
            {
                // Get all saved sessions
                return this.NestResultMultiple(
                    _messageRepo.ListAllSessions());
            }
            catch (System.Exception e)
            {
                return StatusCode(500, e);
            }
        }

        // PUT /Monitor/Sessions/{sessionId}
        [HttpPut("Sessions/{sessionId}")]
        public async Task<IActionResult> UpdateSessionAsync(int sessionId, Session session)
        {
            try
            {
                Session existingSession = _messageRepo.GetSession(sessionId);

                if (existingSession != null)
                {
                    if (!existingSession.Active)
                    {
                        return this.NestResult(-3, "WB_RESULT_SESSION_INACTIVE");
                    } 
                    else
                    {
                        session.CopyTo(existingSession);

                        if (await _messageRepo.UpdateSessionAsync(existingSession))
                        {
                            _logger.LogInformation("Session saved to database");
                                return this.NestResultSingle(existingSession);
                        }
                        else
                        {
                            return this.NestResult(-1, "WB_RESULT_ERROR");
                        }
                    }
                }

                return this.NestResult(-1, "WB_RESULT_SESSION_NOT_FOUND"); 
            }
            catch (System.Exception e)
            {
                return StatusCode(500, e);
            }
        }

        // GET /Monitor/Sessions/{sessionId}/Messages
        [HttpGet("Sessions/{sessionId}/Messages")]
        public IActionResult GetSessionMessages(int sessionId)
        {
            try
            {
                // Messages by archived session
                return this.NestResultMultiple(
                    _messageRepo.ListMessagesBySession(sessionId));
            }
            catch (System.Exception e)
            {
                return StatusCode(500, e);
            }
        }
    }
}
