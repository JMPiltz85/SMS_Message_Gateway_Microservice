using System.ComponentModel.DataAnnotations;

namespace SMS_Service.Models
{

    /*The interface and class are used to log information for the SMS Message request*/

    interface IMessages
    {
        string SourcePhoneNumber { get; set;}
        DateTime RequestTime {get; set;}
        string MessageContent {get; set;}

        string DestPhoneNumber { get; set; }
    }
   

    public class Messages: IMessages
    {
        [Required]
        public string SourcePhoneNumber { get; set;}
        public DateTime RequestTime {get; set;}
        [Required]
        public string MessageContent {get; set;}

        public string DestPhoneNumber { get; set; }

        public Messages()
        {
            this.SourcePhoneNumber = "";
            this.RequestTime= new DateTime();
            this.MessageContent="";
            this.DestPhoneNumber = "";
        }

        public Messages(string _phoneNum, DateTime _requestTime, string _messageContent ="", string _destPhoneNum="" )
        {
            this.SourcePhoneNumber = _phoneNum;
            this.RequestTime= _requestTime;
            this.MessageContent= _messageContent;
            this.DestPhoneNumber = _destPhoneNum;

        }

    }

}