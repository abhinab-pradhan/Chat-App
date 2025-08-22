using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ChatClient
{
    public partial class Form1 : Form
    {
        TcpClient client;
        NetworkStream stream;

        TextBox inputBox;
        Button sendButton;
        RichTextBox chatBox;

        public Form1()
        {
            InitializeComponent();
            SetupUI();
            ConnectToServer();
        }

        private void SetupUI()
        {
            this.Text = "Chat Client";
            this.Width = 400;
            this.Height = 500;

            chatBox = new RichTextBox();
            chatBox.Location = new System.Drawing.Point(10, 10);
            chatBox.Size = new System.Drawing.Size(360, 350);
            chatBox.ReadOnly = true;
            this.Controls.Add(chatBox);

            inputBox = new TextBox();
            inputBox.Location = new System.Drawing.Point(10, 370);
            inputBox.Size = new System.Drawing.Size(260, 30);
            this.Controls.Add(inputBox);

            sendButton = new Button();
            sendButton.Text = "Send";
            sendButton.Location = new System.Drawing.Point(280, 370);
            sendButton.Click += SendButton_Click;
            this.Controls.Add(sendButton);
        }

        private void ConnectToServer()
        {
            try
            {
                client = new TcpClient("127.0.0.1", 5000);
                stream = client.GetStream();

                Thread thread = new Thread(ListenForMessages);
                thread.IsBackground = true;
                thread.Start();

                chatBox.AppendText("Connected to server...\n");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Connection failed: " + ex.Message);
            }
        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            string message = inputBox.Text;
            if (string.IsNullOrEmpty(message)) return;

            byte[] data = Encoding.UTF8.GetBytes(message);
            stream.Write(data, 0, data.Length);

            chatBox.AppendText("Me: " + message + "\n");
            inputBox.Clear();
        }

        private void ListenForMessages()
        {
            byte[] buffer = new byte[1024];
            while (true)
            {
                try
                {
                    int bytes = stream.Read(buffer, 0, buffer.Length);
                    string message = Encoding.UTF8.GetString(buffer, 0, bytes);

                    this.Invoke((MethodInvoker)delegate
                    {
                        chatBox.AppendText("Friend: " + message + "\n");
                    });
                }
                catch { break; }
            }
        }
    }
}
