﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NServiceBus;
using Shared;

namespace ApiApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IMessageSession _messageSession;

        public TestController(IMessageSession messageSession)
        {
            _messageSession = messageSession;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            await _messageSession.Send("Host", new MyFirstMessage
            {
                MessageId = id
            }).ConfigureAwait(false);

            return Ok();
        }
    }
}
