using System.ComponentModel.DataAnnotations;

namespace SMS_Service.Models
{

    /*The interface and class are used to log information for the SMS Message request*/

    interface IMessages
    {
        string PhoneNumber {get; set;}
        DateTime RequestTime {get; set;}
        string MessageContent {get; set;}
    }
   

    public class Messages: IMessages
    {
        [Required]
        public string PhoneNumber {get; set;}
        public DateTime RequestTime {get; set;}
        [Required]
        public string MessageContent {get; set;}

        public Messages()
        {
            this.PhoneNumber="";
            this.RequestTime= new DateTime();
            this.MessageContent="";
        }

        public Messages(string _phoneNum, DateTime _requestTime, string _messageContent ="" )
        {
            this.PhoneNumber=_phoneNum;
            this.RequestTime= _requestTime;
            this.MessageContent= _messageContent;

        }

    }

}