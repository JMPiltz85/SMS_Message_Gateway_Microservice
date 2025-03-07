using Xunit;
using SMS_Service.Models;
using SMS_Service.Services;
using SMS_Service.Controllers;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace SMSMessageUnitTestProject
{
    public class UnitTest1
    {
        private MessageTrackingService _service;
        private MessageController _controller;

        private int maxPhoneMsg;
        private int maxAcctMsg;

        public UnitTest1()
        {
            // Set up configuration to be able to read from asppsettings file
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) // Set the base path for config
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfiguration _configuration = configurationBuilder.Build();
            maxPhoneMsg = _configuration.GetValue<int>("MaximumValues:MaximumPhoneNumberMessages");
            maxAcctMsg = _configuration.GetValue<int>("MaximumValues:MaximumAccountMessages");

            //_service = new MessageTrackingService(3, 5);
            _service = new MessageTrackingService(maxPhoneMsg, maxAcctMsg);
            _controller = new MessageController(_service);
        }

        /*Service Tests*/

        [Fact]
        public async Task Service_One_Message_Valid()
        {
            var output = _service.processMessage("9055551234", DateTime.Now, "Hello World!");

            Assert.True(output.Result, "Could not process the message properly");
        }

        [Fact]
        public async Task Service_Several_Messages_Phone_Valid()
        {
            Task<bool> output = null;
            List<Messages> list = new List<Messages>();
            list.Add(new Messages("9055551234", DateTime.Now, "Hello World!"));
            list.Add(new Messages("9055551234", DateTime.Now, "Good night World!"));
            list.Add(new Messages("9055551234", DateTime.Now, "Good afternoon World!"));

            foreach (Messages m in list)
            {
                output = _service.processMessage(m.SourcePhoneNumber, m.RequestTime, m.MessageContent);
            }

            Assert.True(output.Result, "Could not process the messages properly");
        }

        [Fact]
        public async Task Service_Several_Messages_Phone_Valid_Concurrently()
        {
            List<Messages> list = new List<Messages>();
            list.Add(new Messages("9055551234", DateTime.Now, "Hello World!"));
            list.Add(new Messages("9055551234", DateTime.Now, "Good night World!"));
            list.Add(new Messages("9055551234", DateTime.Now, "Good afternoon World!"));

            var tasks = new Task<bool>[list.Count];

            //sends the requests concurrently
            for(int i=0; i < list.Count; i++)
            {
                tasks[i] = _service.processMessage(list[i].SourcePhoneNumber, list[i].RequestTime, list[i].MessageContent);
            }

            //waits for the tasks to be completed
            var responses = await Task.WhenAll(tasks);

            for(int i =0; i < responses.Length; i++)
            {
                Assert.NotNull(responses[i]);
                Assert.True(responses[i], $"The message sent from phone num {list[i].SourcePhoneNumber} with message {list[i].MessageContent} couldn't be sent");
            }

        }

        [Fact]
        public async Task Service_Several_Messages_Phone_Limit_Reached_Concurrently()
        {
            Task<bool> output = null;
            List<Messages> list = new List<Messages>();
            //list.Add(new Messages("9055551234", DateTime.Now, "Hello World!"));
            //list.Add(new Messages("9055551234", DateTime.Now, "Good night World!"));
            //list.Add(new Messages("9055551234", DateTime.Now, "Good afternoon World!"));
            //list.Add(new Messages("9055551234", DateTime.Now, "Farewell!"));

            for (int i = 0; i <= maxPhoneMsg; i++)
                list.Add(new Messages("9055551234", DateTime.Now, "Hello World!"));

            var tasks = new Task<bool>[list.Count];

            //sends the requests concurrently
            for (int i = 0; i < list.Count; i++)
            {
                tasks[i] = _service.processMessage(list[i].SourcePhoneNumber, list[i].RequestTime, list[i].MessageContent);
            }

            //waits for the tasks to be completed
            var responses = await Task.WhenAll(tasks);

            Assert.True(responses.Contains(false), "Message queued went over limit for same phone number and should have acted accordingly");

        }

        [Fact]
        public async Task Service_Several_Messages_Phone_Limit_Reached_Concurrently2()
        {
            Task<bool> output = null;
            List<Messages> list = new List<Messages>();
            list.Add(new Messages("9055551234", DateTime.Now, "Hello World!"));
            list.Add(new Messages("4165559875", DateTime.Now, "Good night World!"));
            //list.Add(new Messages("6475558754", DateTime.Now, "Good afternoon World!"));
            //list.Add(new Messages("6475558754", DateTime.Now, "I like to order a pizza"));
            //list.Add(new Messages("6475558754", DateTime.Now, "No pickles for me."));
            //list.Add(new Messages("6475558754", DateTime.Now, "Please clean up after the party."));


            for (int i = 0; i <= maxPhoneMsg; i++)
                list.Add(new Messages("6475558754", DateTime.Now, "Good afternoon World!"));

            var tasks = new Task<bool>[list.Count];

            //sends the requests concurrently
            for (int i = 0; i < list.Count; i++)
            {
                tasks[i] = _service.processMessage(list[i].SourcePhoneNumber, list[i].RequestTime, list[i].MessageContent);
            }

            //waits for the tasks to be completed
            var responses = await Task.WhenAll(tasks);

            Assert.True(responses.Contains(false), "Message queued went over limit for same phone number and should have acted accordingly");

        }

        [Fact]
        public async Task Service_Several_Messages_Phone_Limit_Reached_Concurrently3()
        {
            Task<bool> output = null;
            List<Messages> list = new List<Messages>();
            list.Add(new Messages("9055551234", DateTime.Now, "Hello World!"));

            //list.Add(new Messages("6475558754", DateTime.Now, "Good afternoon World!"));
            //list.Add(new Messages("6475558754", DateTime.Now, "I like to order a pizza"));
            //list.Add(new Messages("6475558754", DateTime.Now, "No pickles for me."));
            //list.Add(new Messages("6475558754", DateTime.Now, "Please clean up after the party."));

            for (int i = 0; i <= maxPhoneMsg; i++)
                list.Add(new Messages("6475558754", DateTime.Now, "Good afternoon World!"));

            list.Add(new Messages("4165559875", DateTime.Now, "Good night World!"));

            var tasks = new Task<bool>[list.Count];

            //sends the requests concurrently
            for (int i = 0; i < list.Count; i++)
            {
                tasks[i] = _service.processMessage(list[i].SourcePhoneNumber, list[i].RequestTime, list[i].MessageContent);
            }

            //waits for the tasks to be completed
            var responses = await Task.WhenAll(tasks);

            Assert.True(responses.Contains(false), "The limit for the phone numbers has been reached and the last message should not be processed");

        }

        [Fact]
        public async Task Service_Several_Messages_Acct_Valid()
        {
            Task<bool> output = null;
            List<Messages> list = new List<Messages>();
            list.Add(new Messages("9055551234", DateTime.Now, "Hello World!"));
            list.Add(new Messages("4165559875", DateTime.Now, "Good night World!"));
            list.Add(new Messages("6475558754", DateTime.Now, "Good afternoon World!"));
            list.Add(new Messages("6475558754", DateTime.Now, "I like to order a pizza"));
            list.Add(new Messages("9055551234", DateTime.Now, "No pickles for me."));

            foreach (Messages m in list)
            {
                output = _service.processMessage(m.SourcePhoneNumber, m.RequestTime, m.MessageContent);
            }

            Assert.True(output.Result, "All of the messages in the queue couldn't be processed");
        }

        [Fact]
        public async Task Service_Several_Messages_Acct_Valid_Concurrently()
        {
            Task<bool> output = null;
            List<Messages> list = new List<Messages>();
            //list.Add(new Messages("9055551234", DateTime.Now, "Hello World!"));
            //list.Add(new Messages("4165559875", DateTime.Now, "Good night World!"));
            //list.Add(new Messages("6475558754", DateTime.Now, "Good afternoon World!"));
            //list.Add(new Messages("6475558754", DateTime.Now, "I like to order a pizza"));
            //list.Add(new Messages("9055551234", DateTime.Now, "No pickles for me."));

            for (int i = 1; i <= maxAcctMsg-2; i++)
                list.Add(new Messages(i.ToString(), DateTime.Now, "Good afternoon World!"));

            var tasks = new Task<bool>[list.Count];

            //sends the requests concurrently
            for (int i = 0; i < list.Count; i++)
            {
                tasks[i] = _service.processMessage(list[i].SourcePhoneNumber, list[i].RequestTime, list[i].MessageContent);
            }

            //waits for the tasks to be completed
            var responses = await Task.WhenAll(tasks);

            Assert.False(responses.Contains(false), "Message queue should not be over the limit and thus all messages should be sent");

        }

        [Fact]
        public async Task Service_Several_Messages_Acct_Valid_Dequeue()
        {
            Task<bool> output = null;
            List<Messages> list = new List<Messages>();
            list.Add(new Messages("9055551234", DateTime.Now, "Hello World!"));
            list.Add(new Messages("4165559875", DateTime.Now, "Good night World!"));
            list.Add(new Messages("6475558754", DateTime.Now, "Good afternoon World!"));
            list.Add(new Messages("6475558754", DateTime.Now, "I like to order a pizza"));
            list.Add(new Messages("9055551234", DateTime.Now, "No pickles for me."));
            list.Add(new Messages("4165559875", DateTime.Now, "Please clean up after the party."));

            foreach (Messages m in list)
            {
                output = _service.processMessage(m.SourcePhoneNumber, m.RequestTime, m.MessageContent);

                _service.dequeueMessage();
            }

            Assert.True(output.Result, "All of the messages in the queue couldn't be processed due to dequeue issues");
        }

        [Fact]
        public async Task Service_Several_Messages_Acct_Invalid_Concurrently()
        {
            List<Messages> list = new List<Messages>();
            //list.Add(new Messages("9055551234", DateTime.Now, "Hello World!"));
            //list.Add(new Messages("4165559875", DateTime.Now, "Good night World!"));
            //list.Add(new Messages("6475558754", DateTime.Now, "Good afternoon World!"));
            //list.Add(new Messages("6475558754", DateTime.Now, "I like to order a pizza"));
            //list.Add(new Messages("9055551234", DateTime.Now, "No pickles for me."));
            //list.Add(new Messages("4165559875", DateTime.Now, "Please clean up after the party."));

            for (int i = 0; i <= maxAcctMsg ; i++)
                list.Add(new Messages(i.ToString(), DateTime.Now, "Good afternoon World!"));

            var tasks = new Task<bool>[list.Count];

            //sends the requests concurrently
            for (int i = 0; i < list.Count; i++)
            {
                tasks[i] = _service.processMessage(list[i].SourcePhoneNumber, list[i].RequestTime, list[i].MessageContent);
            }

            //waits for the tasks to be completed
            var responses = await Task.WhenAll(tasks);

            Assert.True(responses.Contains(false), "The limit for the account has been reached and should act according.");
        }

        [Fact]
        public async Task Service_EmptyPhoneNumber()
        {
            var output = _service.processMessage("", DateTime.Now, "Hello World!");

            Assert.False(output.Result, "Phone Number is mandatory, thus message shouldn't be allowed to be sent");
        }

        [Fact]
        public async Task Service_EmptyPhoneNumber_Concurrently()
        {
            Task<bool> output = null;
            List<Messages> list = new List<Messages>();
            list.Add(new Messages("9055551234", DateTime.Now, "Hello World!"));
            list.Add(new Messages("", DateTime.Now, "Good night World!"));
            //list.Add(new Messages("6475558754", DateTime.Now, "Good afternoon World!"));
            //list.Add(new Messages("6475558754", DateTime.Now, "I like to order a pizza"));
            //list.Add(new Messages("9055551234", DateTime.Now, "No pickles for me."));
            //list.Add(new Messages("4165559875", DateTime.Now, "Please clean up after the party."));


            for (int i = 0; i < maxAcctMsg-10; i++)
                list.Add(new Messages(i.ToString(), DateTime.Now, "Good afternoon World!"));

            var tasks = new Task<bool>[list.Count];

            //sends the requests concurrently
            for (int i = 0; i < list.Count; i++)
            {
                tasks[i] = _service.processMessage(list[i].SourcePhoneNumber, list[i].RequestTime, list[i].MessageContent);
            }

            //waits for the tasks to be completed
            var responses = await Task.WhenAll(tasks);

            Assert.True(responses.Contains(false), "Phone Number is mandatory and invalid message shouldn't be sent out.");
        }

        [Fact]
        public async Task Service_EmptyMessageContent()
        {
            var output = _service.processMessage("9055551234", DateTime.Now, "");

            Assert.False(output.Result, "Message Content is mandatory, thus message shouldn't be allowed to be sent");
        }

        [Fact]
        public async Task Service_Queue_Populated()
        {
            var output = await _service.processMessage("9055551234", DateTime.Now, "Hello World!");
            await _service.processMessage("4165559875", DateTime.Now, "Good night World!");

            List<Messages> list = _service.listActiveMessages();

            Assert.True(list.Count() > 0, "Queue should be populated with two messages");

        }

        [Fact]
        public async Task Service_Queue_Dequeued()
        {
            var output = await _service.processMessage("9055551234", DateTime.Now, "Hello World!");

            await _service.dequeueMessage();

            List<Messages> list = _service.listActiveMessages();

            Assert.True(list.Count() == 0, "Queue should be empty");

        }

        [Fact]
        public async Task Service_Queue_QueryByPhone()
        {
            await _service.processMessage("9055551234", DateTime.Now, "Hello World!");
            await _service.processMessage("4165559875", DateTime.Now, "Good night World!");

            List<Messages> list = _service.getLogsByPhoneNum("9055551234");

            Assert.True(list.Count() == 1, "Should get one record by phone number");

        }

        /*Controller Tests*/

        [Fact]
        public async Task Controller_SendOneMessage_Valid()
        {
            Messages msg = new Messages("9055551234", DateTime.Now, "Hello World!");

            var output = await _controller.SendMessage(msg);

            var okResult = Assert.IsType<OkObjectResult>(output);

            Assert.IsType<OkObjectResult>(okResult);
        }

        [Fact]
        public async Task Controller_SendOneMessage_NoPhone()
        {
            Messages msg = new Messages("", DateTime.Now, "Hello World!");

            var output = await _controller.SendMessage(msg);

            var badResult = Assert.IsType<BadRequestObjectResult>(output);

            Assert.IsType<BadRequestObjectResult>(badResult);

        }

        [Fact]
        public async Task Controller_SendOneMessage_NoMessageContent()
        {
            Messages msg = new Messages("9055551234", DateTime.Now, "");

            var output = await _controller.SendMessage(msg);

            var badResult = Assert.IsType<BadRequestObjectResult>(output);

            Assert.IsType<BadRequestObjectResult>(badResult);

        }

        [Fact]
        public async Task Controller_SendMultipleMessages_Valid()
        {

            List<Messages> list = new List<Messages>();

            for (int i = 0; i < maxAcctMsg; i++)
                list.Add(new Messages(i.ToString(), DateTime.Now, "Good afternoon World!"));

            var tasks = new Task<IActionResult>[list.Count];

            //sends the requests concurrently
            for (int i = 0; i < list.Count; i++)
            {
                tasks[i] = _controller.SendMessage(list[i]);
            }

            //waits for the tasks to be completed
            var responses = await Task.WhenAll(tasks);

            //foreach(var item in responses)
            //{
            //    Assert.IsType<OkObjectResult>(item);
            //}

            Assert.All(responses, item => Assert.IsType<OkObjectResult>(item));

        }

        [Fact]
        public async Task Controller_SendMultipleMessages_Invalid()
        {

            List<Messages> list = new List<Messages>();

            for (int i = 0; i <= maxAcctMsg; i++)
                list.Add(new Messages(i.ToString(), DateTime.Now, "Good afternoon World!"));

            var tasks = new Task<IActionResult>[list.Count];

            //sends the requests concurrently
            for (int i = 0; i < list.Count; i++)
            {
                tasks[i] = _controller.SendMessage(list[i]);
            }

            //waits for the tasks to be completed
            var responses = await Task.WhenAll(tasks);

            //checks to see if the list of ActionResults has an element that's of type "BadRequest"
            var hasBadRequest = responses.FirstOrDefault(m => m.GetType() == typeof(BadRequestObjectResult));

            Assert.NotNull(hasBadRequest);
            Assert.IsType<BadRequestObjectResult>(hasBadRequest);

            //bool hasBadRequest = false;
            //foreach(var item in responses)
            //{
            //    if(item.GetType() == typeof(BadRequestObjectResult) ) 
            //    {
            //        hasBadRequest = true;
            //        break;
            //    }
            //}

            //Assert.True(hasBadRequest, "Account Limit wasn't respected");
        }
    }
}