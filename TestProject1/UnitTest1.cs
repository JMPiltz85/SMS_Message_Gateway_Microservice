using Xunit;
using SMS_Service.Models;
using SMS_Service.Services;
using SMS_Service.Controllers;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Microsoft.AspNetCore.Mvc;

namespace SMSMessageUnitTestProject
{
    public class UnitTest1
    {
        private MessageTrackingService _service;
        private MessageController _controller;

        public UnitTest1()
        {
            _service = new MessageTrackingService(3, 5);
            _controller = new MessageController(_service);
        }

        /*Service Tests*/

        [Fact]
        public async Task Service_processMessage_One_Message_Valid()
        {
            var output = _service.processMessage("9055551234", DateTime.Now, "Hello World!");

            Assert.True(output.Result, "Could not process the message properly");
        }

        [Fact]
        public async Task Service_processMessage_Several_Messages_Phone_Valid()
        {
            Task<bool> output = null;
            List<Messages> list = new List<Messages>();
            list.Add(new Messages("9055551234", DateTime.Now, "Hello World!"));
            list.Add(new Messages("9055551234", DateTime.Now, "Good night World!"));
            list.Add(new Messages("9055551234", DateTime.Now, "Good afternoon World!"));

            foreach (Messages m in list)
            {
                output = _service.processMessage(m.PhoneNumber, m.RequestTime, m.MessageContent);
            }

            Assert.True(output.Result, "Could not process the messages properly");
        }


        [Fact]
        public async Task Service_processMessage_Several_Messages_Phone_Limit_Reached()
        {
            Task<bool> output = null;
            List<Messages> list = new List<Messages>();
            list.Add(new Messages("9055551234", DateTime.Now, "Hello World!"));
            list.Add(new Messages("9055551234", DateTime.Now, "Good night World!"));
            list.Add(new Messages("9055551234", DateTime.Now, "Good afternoon World!"));
            list.Add(new Messages("9055551234", DateTime.Now, "Farewell!"));

            foreach (Messages m in list)
            {
                output = _service.processMessage(m.PhoneNumber, m.RequestTime, m.MessageContent);

                if (output.Result == false)
                    break;
            }

            Assert.False(output.Result, "Message queued went over limit for same phone number");
        }

        [Fact]
        public async Task Service_processMessage_Several_Messages_Phone_Limit_Reached2()
        {
            Task<bool> output = null;
            List<Messages> list = new List<Messages>();
            list.Add(new Messages("9055551234", DateTime.Now, "Hello World!"));
            list.Add(new Messages("4165559875", DateTime.Now, "Good night World!"));
            list.Add(new Messages("6475558754", DateTime.Now, "Good afternoon World!"));
            list.Add(new Messages("6475558754", DateTime.Now, "I like to order a pizza"));
            list.Add(new Messages("6475558754", DateTime.Now, "No pickles for me."));
            list.Add(new Messages("6475558754", DateTime.Now, "Please clean up after the party."));

            foreach (Messages m in list)
            {
                output = _service.processMessage(m.PhoneNumber, m.RequestTime, m.MessageContent);

                if (output.Result == false)
                    break;
            }

            Assert.False(output.Result, "The limit for the phone numbers has been reached and the last message should not be processed");
        }

        [Fact]
        public async Task Service_processMessage_Several_Messages_Phone_Limit_Reached3()
        {
            Task<bool> output = null;
            List<Messages> list = new List<Messages>();
            list.Add(new Messages("9055551234", DateTime.Now, "Hello World!"));
            list.Add(new Messages("6475558754", DateTime.Now, "Good afternoon World!"));
            list.Add(new Messages("6475558754", DateTime.Now, "I like to order a pizza"));
            list.Add(new Messages("6475558754", DateTime.Now, "No pickles for me."));
            list.Add(new Messages("6475558754", DateTime.Now, "Please clean up after the party."));
            list.Add(new Messages("4165559875", DateTime.Now, "Good night World!"));

            foreach (Messages m in list)
            {
                output = _service.processMessage(m.PhoneNumber, m.RequestTime, m.MessageContent);

                if (output.Result == false)
                    break;
            }

            Assert.False(output.Result, "The limit for the phone numbers has been reached and the last message should not be processed");
        }

        [Fact]
        public async Task Service_processMessage_Several_Messages_Acct_Valid()
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
                output = _service.processMessage(m.PhoneNumber, m.RequestTime, m.MessageContent);
            }

            Assert.True(output.Result, "All of the messages in the queue couldn't be processed");
        }

        [Fact]
        public async Task Service_processMessage_Several_Messages_Acct_Valid_Dequeue()
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
                output = _service.processMessage(m.PhoneNumber, m.RequestTime, m.MessageContent);

                _service.dequeueMessage();
            }

            Assert.True(output.Result, "All of the messages in the queue couldn't be processed due to dequeue issues");
        }

        [Fact]
        public async Task Service_processMessage_Several_Messages_Acct_Invalid()
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
                output = _service.processMessage(m.PhoneNumber, m.RequestTime, m.MessageContent);

                if (output.Result == false)
                    break;
            }

            Assert.False(output.Result, "The limit for the account has been reached and the last message should not be processed");
        }

        [Fact]
        public async Task Service_processMessage_EmptyPhoneNumber()
        {
            var output = _service.processMessage("", DateTime.Now, "Hello World!");

            Assert.False(output.Result, "Phone Number is mandatory, thus message shouldn't be allowed to be sent");
        }

        [Fact]
        public async Task Service_processMessage_EmptyMessageContent()
        {
            var output = _service.processMessage("9055551234", DateTime.Now, "");

            Assert.False(output.Result, "Message Content is mandatory, thus message shouldn't be allowed to be sent");
        }

        [Fact]
        public async Task Service_processMessage_Queue_Populated()
        {
            var output = await _service.processMessage("9055551234", DateTime.Now, "Hello World!");
            await _service.processMessage("4165559875", DateTime.Now, "Good night World!");

            List<Messages> list = _service.listActiveMessages();

            Assert.True(list.Count() > 0, "Queue should be populated with two messages");

        }

        [Fact]
        public async Task Service_processMessage_Queue_Dequeued()
        {
            var output = await _service.processMessage("9055551234", DateTime.Now, "Hello World!");

            await _service.dequeueMessage();

            List<Messages> list = _service.listActiveMessages();

            Assert.True(list.Count() == 0, "Queue should be empty");

        }

        [Fact]
        public async Task Service_processMessage_Queue_QueryByPhone()
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
    }
}