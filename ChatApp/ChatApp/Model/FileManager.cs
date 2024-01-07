using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text.Json;

namespace ChatApp.Model
{
    public class FileManager
    {
        public ObservableCollection<ChatLogModel>? _chatLogs;
        readonly string filePath;

        public FileManager()
        {
            filePath = Path.Combine(Directory.GetCurrentDirectory(), "db.json");
        }

        // Method for saving a chat log.
        public bool SaveChatToFile(ObservableCollection<MessageModel> data, string serverName, string clientName)
        {
            try
            {
                string MessageDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string chatLogName = serverName + "-" + clientName + "_" + MessageDateTime;

                ChatLogModel NewChatLog = new ChatLogModel(chatLogName, data);

                ObservableCollection<ChatLogModel> jsonObject;
                if (File.Exists(filePath))
                {
                    string jsonContent = File.ReadAllText(filePath);
                    var TempJsonObject = JsonSerializer.Deserialize<ObservableCollection<ChatLogModel>>(jsonContent);
                    if (TempJsonObject != null)
                    {
                        jsonObject = TempJsonObject;
                    }
                    else
                    {
                        jsonObject = new ObservableCollection<ChatLogModel>();
                    }
                }
                else
                {
                    jsonObject = new ObservableCollection<ChatLogModel>();
                }

                jsonObject.Add(NewChatLog);

                string updatedJson = JsonSerializer.Serialize(jsonObject);

                File.WriteAllText(filePath, updatedJson);
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
                return false;
            }
            return true;
        }

        //Function for reading and updating _chatLogs. If no file can be found, an empty file is created.
        public void ReadDatabase()
        {
            try
            {
                if (File.Exists(filePath))
                {
                    string jsonContent = File.ReadAllText(filePath);
                    var TempJsonObject = JsonSerializer.Deserialize<ObservableCollection<ChatLogModel>>(jsonContent);

                    if (TempJsonObject != null)
                    {
                        _chatLogs = TempJsonObject;
                    }
                    else
                    {
                        _chatLogs = new ObservableCollection<ChatLogModel>();
                        string jsonString = JsonSerializer.Serialize(_chatLogs);
                        File.WriteAllText(filePath, jsonString);
                    }
                }
                else
                {
                    _chatLogs = new ObservableCollection<ChatLogModel>();
                    string jsonString = JsonSerializer.Serialize(_chatLogs);
                    File.WriteAllText(filePath, jsonString);
                    throw new FileNotFoundException("File not found");
                }
            }
            catch (Exception exx)
            {
                Debug.WriteLine(exx.ToString());
            }
        }
    }
}
