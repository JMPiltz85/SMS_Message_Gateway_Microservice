namespace SMS_Message_Gateway_Microservice.Models
{
    public class MaximumSettings
    {
        public int MaximumPhoneNumberMessages { get; set; }
        public int MaximumAccountMessages { get; set; }

        public MaximumSettings()
        {
            this.MaximumPhoneNumberMessages = 0;
            this.MaximumAccountMessages = 0;
        }

        public MaximumSettings(int _maxPhoneNumMsg, int _maxAcctMsg)
        {
            this.MaximumPhoneNumberMessages = _maxPhoneNumMsg;
            this.MaximumAccountMessages = _maxAcctMsg;
        }

    }
}
