namespace MsgInfo
{
    public class MessageInfo
    {
        public string Message { get; set; }
        public string Name { get; set; }
        public DateTime Time { get; set; }
        public bool Flag { get; set; }
        public string Connection { get; set; }

        public MessageInfo(string message, string name)
        {
            this.Message = message;
            Time = DateTime.Now;
            Name = name;

        }

        public override string ToString()
        {
            return $" {Name} -- {Message} ({Time.ToShortTimeString()})";
        }
    }
}