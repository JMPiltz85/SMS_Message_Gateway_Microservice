using SMS_Service.Models;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Concurrent;

namespace SMS_Service.Services
{


    public interface IMessageTrackingService
    {

        Task<bool> processMessage(string PhoneNumber, DateTime timeSent, string Message);
        int getMessageRecordAmt(string phoneNumber);
        Task<Messages> dequeueMessage();

        List<Messages> getLogsByPhoneNum(string phoneNumber);
        List<Messages> listActiveMessages();
    }

    public class MessageTrackingService: IMessageTrackingService
    {
        private ConcurrentQueue<Messages> msgQueue {get; set;}
        private int maxCallAmtPhoneNum {get; set;}
        private int maxCallAmtAccount {get; set;}

        public MessageTrackingService(int maxPhoneCalls=0, int maxAccountCalls =0)
        {
            this.msgQueue = new ConcurrentQueue<Messages>();
            this.maxCallAmtPhoneNum = maxPhoneCalls;
            this.maxCallAmtAccount = maxAccountCalls;
        }

        
        //checks to see if the message can be sent. If so, it's added to the message list
        public async Task<bool> processMessage(string PhoneNumber, DateTime timeSent, string Message ="")
        {
            Messages log = new Messages(PhoneNumber, timeSent, Message);
            bool canSend = canSendMessage( log);

            if(canSend)
                msgQueue.Enqueue(log);
            
            return canSend;
        }

        //gets the current amount of records pertaining to the phone number
        public int getMessageRecordAmt(string phoneNumber)
        {
            int amount = msgQueue.Where(m => m.PhoneNumber == phoneNumber).Count();

            return amount;
        }

        //NOTE: Sees which messages are still in the message queue
        public List<Messages> getLogsByPhoneNum(string phoneNumber)
        {
            return msgQueue.Where(m => m.PhoneNumber == phoneNumber).ToList();
        }

        public List<Messages> listActiveMessages()
        {
            return msgQueue.ToList();
        }

        //NOTE: removes an message from the front of the queue and returns it to simulate processing the message in the queue. 
        public async Task<Messages> dequeueMessage()
        {
            Messages msg = new Messages();
            bool result = false;

            //will keep attempting to dequeue message until it's successful.
            while(!result)
            {
                if(msgQueue.Count > 0)
                {
                    result = msgQueue.TryDequeue(out msg);
                }
            }

            return msg;
        }

        //checks to see if the message is valid, and doesn't go over the limits set for messages that can be sent in a second
        protected bool canSendMessage(Messages log)
        {
            bool canSend = true;
            int currentAmtPhoneNum = getMessageRecordAmt(log.PhoneNumber);
            int currentAmtAccount = msgQueue.Count();

            //NOTE: Kept maximum values in appsettings so can adjust values based on environment
            if(!string.IsNullOrEmpty(log.PhoneNumber) && !string.IsNullOrEmpty(log.MessageContent)  && 
                    currentAmtPhoneNum <  maxCallAmtPhoneNum && currentAmtAccount < maxCallAmtAccount )
                canSend = true;
            else
                canSend = false;

            return canSend;
        }

    }


}