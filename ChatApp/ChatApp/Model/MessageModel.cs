using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ChatApp.Model
{
    public class MessageModel
    {
        public MessageType RequestType { get; set; }
        public string Message { get; set; }
        public string MessageDateTime { get; set; }
        public string Sender { get; set; }

        public MessageModel(MessageType type, string message, string sender)
        {
            RequestType = type;
            Message = message;
            MessageDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            Sender = sender;
        }
        [JsonConstructor]
        public MessageModel(MessageType requestType, string message, string messageDateTime, string sender)
        {
            RequestType = requestType;
            Message = message;
            MessageDateTime = messageDateTime;
            Sender = sender;
        }

        public string ToJson()
        {
            string jsonString = JsonSerializer.Serialize(this);
            return jsonString;
        }
    }
}
