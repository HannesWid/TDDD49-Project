using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Collections.ObjectModel;
using System.Media;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ChatApp.Model
{
    public class NetworkManager : INotifyPropertyChanged
    {
        private TcpListener? listener;
        private TcpClient? client;
        private NetworkStream? stream;

        public bool _IsServer = false;
        private bool isListening = false;
        private bool connectionLive = false;

        private string _ErrorMessage = "";
        private string _OtherUser = "";
        private string _LatestUser = "";


        private int Port { get; set; }
        private IPAddress? Address { get; set; }

        public bool IsServer
        {
            get
            {
                return _IsServer;
            }
            set
            {
                if (_IsServer != value)
                {
                    _IsServer = value;
                    OnPropertyChanged(nameof(IsServer));
                }
            }
        }

        public string OtherUser
        {
            get
            {
                if (string.IsNullOrEmpty(_OtherUser))
                {
                    return "No user in chat";
                }
                return _OtherUser;
            }
            set
            {
                if (_OtherUser != value)
                {
                    _OtherUser = value;
                    OnPropertyChanged(nameof(OtherUser));
                }
            }
        }

        public string LatestUser
        {
            get
            {
                return _LatestUser;
            }
            set
            {
                if (_LatestUser != value)
                {
                    _LatestUser = value;
                    OnPropertyChanged(nameof(LatestUser));
                }
            }
        }

        public string ErrorMessage
        {
            get
            {
                return _ErrorMessage;
            }
            set
            {
                if (_ErrorMessage != value)
                {
                    _ErrorMessage = value;
                    OnPropertyChanged(nameof(ErrorMessage));
                }
            }
        }

        public ObservableCollection<MessageModel> _Messages;

        public NetworkManager()
        {
            _Messages = new ObservableCollection<MessageModel>();
        }

        public async Task<bool> StartConnection(UserModel user)
        {
            return await Task.Run(() =>
            {
                ClearMessages();
                LatestUser = "";
                System.Diagnostics.Debug.WriteLine("Starting a connection...");
                try
                {
                    Port = Int32.Parse(user.PortNumber);
                    Address = IPAddress.Parse(user.IP);
                    listener = new TcpListener(Address, Port);
                    listener.Start();
                }
                catch (FormatException e)
                {
                    Console.Write(e);
                    ErrorMessage = $"Wrongly formatted ip or port, please try again.";
                    listener?.Stop();
                    return false;
                }
                catch (ArgumentNullException e)
                {
                    Console.Write(e);
                    ErrorMessage = $"Wrongly formatted ip or port, please try again.";
                    listener?.Stop();
                    return false;
                }
                catch (OverflowException e)
                {
                    Console.Write(e);
                    ErrorMessage = $"Wrongly formatted ip, please try again.";
                    listener?.Stop();
                    return false;
                }
                catch (SocketException e)
                {
                    Console.Write(e);
                    ErrorMessage = $"Socket could not be established";
                    listener?.Stop();
                    return false;

                }
                catch (ArgumentOutOfRangeException e)
                {
                    Console.Write(e);
                    ErrorMessage = e.Message;
                    listener?.Stop();
                    return false;
                }
                IsServer = true;
                isListening = true;
                return true;
            });
        }


        public async Task<string> GetConnectingUser()
        {
            return await Task.Run(() =>
            {
                if (listener != null)
                {
                    while (isListening)
                    {
                        try
                        {
                            client = listener.AcceptTcpClient();
                            System.Diagnostics.Debug.WriteLine("Client connected.");

                            stream = client.GetStream();
                            byte[] buffer = new byte[1024];
                            int bytesRead = stream.Read(buffer, 0, buffer.Length);
                            string jsonString = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                            System.Diagnostics.Debug.WriteLine($"Received: {jsonString}");
                            // Deserialize the JSON-like string into a C# object
                            MessageModel? msg = JsonSerializer.Deserialize<MessageModel>(jsonString);
                            if (msg != null)
                            {
                                return msg.Sender;
                            }
                            else
                            {
                                client.Close();
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    }
                    return "";
                }
                return "";
            });
        }

        public async Task AnswerConnectionRequest(string sender, UserModel user, bool accept)
        {
            await Task.Run(() =>
            {
                MessageModel model = new MessageModel(MessageType.RejectConnection, user.UserName, user.UserName);
                if (accept)
                {
                    connectionLive = true;
                    model.RequestType = MessageType.AcceptConnection;
                    OtherUser = sender;
                    LatestUser = sender;
                }
                try
                {
                    string answer = JsonSerializer.Serialize(model);
                    byte[] answer_data = System.Text.Encoding.UTF8.GetBytes(answer);
                    stream?.Write(answer_data, 0, answer_data.Length);
                    Debug.WriteLine($"Sent: {answer}");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            });
        }

        public async Task<bool> SendMessage(MessageModel message)
        {
            return await Task.Run(() =>
            {
                if (stream == null) { return false; }
                try
                {
                    string jsonMessage = message.ToJson();
                    byte[] data = System.Text.Encoding.UTF8.GetBytes(jsonMessage);
                    stream.Write(data, 0, data.Length);
                    if (message.RequestType == MessageType.Message)
                    {
                        AddMessage(message);
                    }
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return false;

                }
            });
        }

        public async Task CloseServer(UserModel user)
        {
            await Task.Run(async () =>
            {
                if (connectionLive)
                {
                    connectionLive = false;
                    await SendMessage(new MessageModel(MessageType.TeardownConnection, "User disconnected", user.UserName));
                }
                isListening = false;
                stream?.Close();
                client?.Close();
                listener?.Stop();
            });
        }

        public async Task CloseConnection(UserModel user)
        {
            await SendMessage(new MessageModel(MessageType.TeardownConnection, "User disconnected", user.UserName));
            stream?.Close();
            client?.Close();
            OtherUser = "";
            connectionLive = false;
        }

        public void ConfirmCloseConnection(UserModel user)
        {
            stream?.Close();
            client?.Close();
            OtherUser = "";
            connectionLive = false;
        }

        public async Task<bool> ConnectToServer(UserModel user)
        {
            return await Task.Run(() =>
            {
                ClearMessages();
                LatestUser = "";
                try
                {
                    Port = Int32.Parse(user.PortNumber);
                    client = new TcpClient(user.IP, Port);
                }
                catch (ArgumentNullException e)
                {
                    Console.Write(e);
                    ErrorMessage = $"Wrongly formatted ip or port, please try again.";
                    return false;
                }
                catch (FormatException e)
                {
                    Console.Write(e);
                    ErrorMessage = $"Wrongly formatted ip or port, please try again.";
                    return false;
                }
                catch (OverflowException e)
                {
                    Console.Write(e);
                    ErrorMessage = $"Wrongly formatted ip, please try again.";
                    return false;
                }
                catch (ArgumentOutOfRangeException e)
                {
                    Console.Write(e);
                    ErrorMessage = $"Wrongly formatted ip or port, please try again.";
                    return false;
                }
                catch (SocketException e)
                {
                    Console.Write(e);
                    ErrorMessage = $"Socket could not be established";
                    return false;

                }
                MessageModel message = new MessageModel(MessageType.EstablishConnection, "", user.UserName);
                string msg = JsonSerializer.Serialize(message);
                // Create a TcpClient to connect to the server


                Console.WriteLine("Connected to the server.");

                // Get the network stream for reading and writing data
                stream = client.GetStream();

                // Send a message to the server

                byte[] data = System.Text.Encoding.UTF8.GetBytes(msg);
                stream.Write(data, 0, data.Length);
                Console.WriteLine($"Sent: {msg}");

                bool requestAnswer = false;
                while (!requestAnswer)
                {
                    byte[] buffer = new byte[1024];
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    string jsonString = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    MessageModel? answer = JsonSerializer.Deserialize<MessageModel>(jsonString);
                    if (answer != null && answer.RequestType == MessageType.AcceptConnection)
                    {
                        connectionLive = true;
                        OtherUser = answer.Sender;
                        LatestUser = answer.Sender;
                        return true;
                    }
                    else if (answer != null && answer.RequestType == MessageType.RejectConnection)
                    {
                        ErrorMessage = "Server declined your chat request";
                        return false;
                    }
                    else
                    {
                        ErrorMessage = "Could not connect to server";
                        return false;
                    }
                }
                return true;
            });
        }

        public async Task ListenForMessages(UserModel user)
        {
            await Task.Run(() =>
            {
                ClearMessages();
                while (connectionLive && stream != null)
                {
                    try
                    {
                        byte[] buffer = new byte[1024];
                        int bytesRead = stream.Read(buffer, 0, buffer.Length);
                        string jsonString = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        if (jsonString != null)
                        {
                            MessageModel? msg = JsonSerializer.Deserialize<MessageModel>(jsonString);
                            if (msg != null && msg.RequestType == MessageType.Message)
                            {
                                AddMessage(msg);
                            }
                            else if (msg != null && msg.RequestType == MessageType.TeardownConnection)
                            {
                                OtherUser = "";
                                ConfirmCloseConnection(user);
                            }
                            else if (msg != null && msg.RequestType == MessageType.Buzz)
                            {
                                SystemSounds.Beep.Play();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        connectionLive = false;
                        OtherUser = "";
                        stream.Close();
                        client?.Close();
                        Console.WriteLine(ex);
                    }
                }
            });
        }

        private void AddMessage(MessageModel msg)
        {
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                _Messages.Add(msg);
            });
        }

        private void ClearMessages()
        {
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                _Messages.Clear();
            });
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
