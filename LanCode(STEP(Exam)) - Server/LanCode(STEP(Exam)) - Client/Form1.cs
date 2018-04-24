using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LanCode_STEP_Exam_____Client
{
    public partial class MainForm : Form
    {
        private TcpClient client;
        private BinaryReader binReader;
        private BinaryWriter binWriter;
        private int countOfMessages = 0;

        public MainForm()
        {
            InitializeComponent();
            sendButton.Enabled = false;
            messageTextBox.Enabled = false;
            try
            {
                client = new TcpClient("127.0.0.1", 8888);
                binReader = new BinaryReader(client.GetStream());
                binWriter = new BinaryWriter(client.GetStream());
                new Thread(RecvThread).Start();
            }
            catch
            {
                MessageBox.Show("Can't make the connection", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddChatMessage(string _name, string _message)
        {
            messagesListBox.Items.Add(string.Format("{0}: {1}", _name, _message));
            messagesListBox.SelectedIndex = messagesListBox.Items.Count - 1;
            messagesListBox.SelectedIndex = -1;
        }

        private void RecvThread()
        {
            try
            {
                for (; true;)
                {
                    string message = binReader.ReadString();
                    string[] messageParams = message.Split('|');
                    if (messageParams[0] == "chat")
                    {
                        AddChatMessage(messageParams[1], messageParams[2]);
                    }
                    if (messageParams[0] == "users")
                    {
                        AddUsers(messageParams[1]);
                    }
                }
            }
            catch
            {
                return;
            }
        }

        private void AddUsers(string _name)
        {
            if (LoginCheck(_name))
            {
                usersListBox.Items.Add(string.Format("{0}", _name));
            }
        }

        private void LoginButtonClick(object sender, EventArgs e)
        {
            if(LoginCheck(loginTextBox.Text))
            {
                loginButton.Enabled = false;
                loginTextBox.Enabled = false;
                sendButton.Enabled = true;
                messageTextBox.Enabled = true;
            }
            else
            {
                MessageBox.Show("User with this nickname already in the chat. Choose another one.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public bool LoginCheck(string _name)
        {
            bool isNew = true;
            for (int i = 0; i < usersListBox.Items.Count; ++i)
            {
                if (usersListBox.Items[i].Equals(_name))
                {
                    isNew = false;
                }
            }

            return isNew;
        }

        private void SendButtonClick(object sender, EventArgs e)
        {
            AddChatMessage(loginTextBox.Text, messageTextBox.Text);
            string message = string.Format("chat|{0}|{1}", loginTextBox.Text, messageTextBox.Text);
            binWriter.Write(message);
            message = string.Format("users|{0}", loginTextBox.Text);
            binWriter.Write(message);
            messageTextBox.Text = "";
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            client.Close();
        }
    }
}
