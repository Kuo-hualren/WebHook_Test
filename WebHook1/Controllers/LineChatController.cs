using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebHook1.Domain;
using WebHook1.Dtos.Webhook;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace WebHook1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LineChatController : ControllerBase
    {

        private readonly LineBotService _lineBotService;
        // constructor
        public LineChatController()
        {
            _lineBotService = new LineBotService();
        }

        [Microsoft.AspNetCore.Mvc.HttpPost("Webhook")]
        public IActionResult Webhook(WebhookRequestBodyDto body)
        {
            _lineBotService.ReceiveWebhook(body); // 呼叫 Service
            return Ok();
        }

    }
}