using System.Windows.Forms;

namespace Server
{
    public partial class FormServer : Form
    {
        private ChatServer _server = new ChatServer();

        public FormServer()
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

        private void buttonStart_Click(object sender, System.EventArgs e)
        {
            _server.StartServer(System.Convert.ToInt32(textPort.Text));
        }

        private void buttonClose_Click(object sender, System.EventArgs e)
        {
            _server.EndServer();
        }

        private void FormServer_FormClosed(object sender, FormClosedEventArgs e)
        {
            _server.EndServer();
        }
    }
}
