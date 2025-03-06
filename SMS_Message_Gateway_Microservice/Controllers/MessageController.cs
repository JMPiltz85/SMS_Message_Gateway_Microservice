using Microsoft.AspNetCore.Mvc;
using SMS_Service.Models;
using SMS_Service.Services;

namespace SMS_Service.Controllers
{

    [Route("MessageService")]
    [ApiController]
    public class MessageController: ControllerBase
    {
        private IMessageTrackingService service;

        // Inject the MessageTrackingService service into the controller
        public MessageController(IMessageTrackingService _service){
            this.service = _service;
        }

        [Route("")]
        [Route("Index")]
        [HttpGet]
        public IActionResult Index()
        {
            return Ok("Message Service Index");
        }

        [HttpGet("GetByPhone/{phoneNumber}")]
        public List<Messages> getByPhone(string phoneNumber)
        {
            List<Messages> list = service.getLogsByPhoneNum(phoneNumber);

            return list;
        }

        [HttpGet("GetActiveMessages")]
        public List<Messages> getActiveMessages()
        {
            List<Messages> list = service.listActiveMessages();

            return list;
        }

        [HttpPost("SendMessage")]
        public async Task<IActionResult> SendMessage([FromBody] Messages message)
        {
            bool hasSent = false;
            DateTime dateSent = DateTime.Now;

            //checks to see if phone number is empty
            if(string.IsNullOrEmpty(message.PhoneNumber))
            {
                return BadRequest("Phone number required.");
            }

            if(string.IsNullOrEmpty(message.MessageContent))
            {
                return BadRequest("Message Content is required.");
            }
            
            hasSent = await service.processMessage(message.PhoneNumber,dateSent , message.MessageContent);

            if(!hasSent)
            {
                return BadRequest("Provider's limits on sending SMS messages has been reached. Try again when there is less messages being sent");    
            }
            else{
                await service.dequeueMessage();
            }


            return Ok("Message has been sent.");
        }


    }



}
