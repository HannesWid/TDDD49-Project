using System.Collections.ObjectModel;
using System.Text.Json.Serialization;


namespace ChatApp.Model
{
    public class ChatLogModel
    {
        public string ChatLogName { get; set; }
        public ObservableCollection<MessageModel> Messages { get; set; }

        [JsonConstructor]
        public ChatLogModel(string chatLogName, ObservableCollection<MessageModel> messages)
        {
            ChatLogName = chatLogName;
            Messages = messages;
        }
    }
}
