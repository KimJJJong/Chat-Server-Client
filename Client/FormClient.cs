namespace Client
{
    public partial class FormClient : Form
    {
        private ChatClient _client = new ChatClient();

        public FormClient()
        {
            InitializeComponent();
        }

        public void Message(string message)
        {
            // 메인스레드에서 호출되었는지 확인
            if (textMessage.InvokeRequired)
            {
                textMessage.BeginInvoke(new System.Action(() => textMessage.Text += message + "\r\n"));
            }
            else
            {
                textMessage.Text += message + "\r\n";
            }
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            _client.StartClient(textIP.Text, System.Convert.ToInt32(textPort.Text));
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            SendChat();
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            _client.EndClient();
        }

        private void textInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SendChat();
            }
        }

        private void SendChat()
        {
            _client.Send(textID.Text, textInput.Text);
            textInput.Text = string.Empty;
        }
    }
}
