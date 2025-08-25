using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;

namespace ChatClient
{
    public partial class Form1 : Form
    {
        TcpClient client;
        NetworkStream stream;

        TextBox inputBox;
        Button sendButton;
        FlowLayoutPanel chatPanel;

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
            this.Height = 600;

            // Chat panel
            chatPanel = new FlowLayoutPanel();
            chatPanel.Location = new Point(10, 10);
            chatPanel.Size = new Size(360, 480);
            chatPanel.AutoScroll = true;
            chatPanel.FlowDirection = FlowDirection.TopDown;
            chatPanel.WrapContents = false;
            this.Controls.Add(chatPanel);

            // Input box
            inputBox = new TextBox();
            inputBox.Location = new Point(10, 500);
            inputBox.Size = new Size(260, 30);
            this.Controls.Add(inputBox);

            // Send button
            sendButton = new Button();
            sendButton.Text = "Send";
            sendButton.Location = new Point(280, 500);
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

                AddMessage("Connected to server...", false);
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

            AddMessage(message, true);
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
                        AddMessage(message, false);
                    });
                }
                catch { break; }
            }
        }

        private void AddMessage(string text, bool isMine)
        {
            Label msgLabel = new Label();
            msgLabel.Text = text;
            msgLabel.AutoSize = true;
            msgLabel.MaximumSize = new Size(200, 0);
            msgLabel.Padding = new Padding(8);
            msgLabel.Margin = new Padding(3);

            if (isMine)
            {
                msgLabel.BackColor = Color.LightGreen;
                msgLabel.TextAlign = ContentAlignment.MiddleRight;
                msgLabel.Anchor = AnchorStyles.Right;
                msgLabel.Left = chatPanel.Width - msgLabel.Width - 30;
            }
            else
            {
                msgLabel.BackColor = Color.LightGray;
                msgLabel.TextAlign = ContentAlignment.MiddleLeft;
                msgLabel.Anchor = AnchorStyles.Left;
            }

            chatPanel.Controls.Add(msgLabel);
            chatPanel.ScrollControlIntoView(msgLabel);
        }
    }
}
