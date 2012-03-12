using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Deploy.King.Host.Infrastructure;
using Deploy.King.Host.Infrastructure.Poll;

namespace Deploy.King.Host.Controllers
{
    [Authorize]
    public class PollController : AsyncController
    {
        public JsonResult Negotiate()
        {
            return Json(Guid.NewGuid());
        }

        public ActionResult Subscribe(Guid connectionId, string channel, string group)
        {
            Bus.Subscribe(connectionId, channel, @group);
            return Json(new { });
        }

        public ActionResult Unsubscribe(Guid connectionId, string channel, string group)
        {
            Bus.Unsubscribe(connectionId, channel, @group);
            return Json(new { });
        }

        public void ConnectAsync(Guid connectionId)
        {
            Bus.Connect(connectionId, AsyncManager);
        }

        public ActionResult ConnectCompleted(List<Message> messages)
        {
            if (messages != null)
                return Json(messages.Select(x => new
                {
                    channel = x.Channel,
                    group = x.Group,
                    data = x.Data
                }).ToArray());

            return Json(new List<Message>());
        }

        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new JsonNetResult
            {
                Data = data, 
                ContentType = contentType, 
                ContentEncoding = contentEncoding
            };
        }
    }
}