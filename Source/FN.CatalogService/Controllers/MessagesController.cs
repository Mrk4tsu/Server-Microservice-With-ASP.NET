using FN.Application.Catalog.Product.Notifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace FN.CatalogService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private IHubContext<NotifyHub, ITypedHubClient> _hubContext;
        public MessagesController(IHubContext<NotifyHub, ITypedHubClient> hubContext)
        {
            _hubContext = hubContext;
        }
        [HttpGet, AllowAnonymous]
        public string Get()
        {
            string retMessage = string.Empty;
            //var message = new Message()
            //{
            //    Type = "warning",
            //    Information = "test message " + Guid.NewGuid().ToString()
            //};
            //try
            //{
            //    _hubContext.Clients.All.BroadcastMessage(message);
            //    retMessage = "Success";
            //}
            //catch (Exception e)
            //{
            //    retMessage = e.ToString();
            //}
            return retMessage;
        }
        [HttpPost, AllowAnonymous]
        public string Post(Message message)
        {
            string retMessage = string.Empty;
            //try
            //{
            //    _hubContext.Clients.All.SendMessage(message);
            //    retMessage = "Success";
            //}
            //catch (Exception e)
            //{
            //    retMessage = e.ToString();
            //}
            return retMessage;
        }
    }
}
