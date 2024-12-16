using Client;
using System.Net;
using System.Net.Sockets;

public class ChatClient
{
    private bool _run;
    private Socket _socket;
    private Thread _recvThread;
    private byte[] _recvBuffer = new byte[1024];
    private byte[] _sendBuffer = new byte[1024];
    private MessageResolver _messageResolver;
    private SocketAsyncEventArgs _receiveArgs;
    private SocketAsyncEventArgs _sendArgs;
    private List<byte[]> _sendList = new List<byte[]>(); // Send할 바이트배열의 리스트

    public void StartClient(string ip, int port)
    {
        if (_run)
        { 
            return; 
        }

        try
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress addr = IPAddress.Parse(ip);
            IPEndPoint endPoint = new IPEndPoint(addr, port);
            _socket.Connect(endPoint);
            SetMessage("서버 접속 완료");
        }
        catch (Exception e)
        {
            SetMessage("서버 접속오류 " + e.Message);
            return;
        }

        _messageResolver = new MessageResolver(2048);

        // 비동기 Receive
        _receiveArgs = new SocketAsyncEventArgs();
        _receiveArgs.SetBuffer(_recvBuffer, 0, _recvBuffer.Length);
        _receiveArgs.UserToken = _socket; // 접속자에 대한 정보
        _receiveArgs.Completed += new System.EventHandler<SocketAsyncEventArgs>(OnReceiveCompleted);
        StartReceive();

        // 비동기 Send
        _sendArgs = new SocketAsyncEventArgs();
        _sendArgs.SetBuffer(_sendBuffer, 0, _sendBuffer.Length);
        _sendArgs.UserToken = _socket;
        _sendArgs.Completed += new System.EventHandler<SocketAsyncEventArgs>(OnSendCompleted);

        //// 전송 받기 위한 스레드
        //_recvThread = new Thread(ReceiveThread);
        //_recvThread.Start();

        _run = true;
    }

    public void EndClient()
    {
        if (!_run)
            return;

        _socket.Close();
        _run = false;
    }

    private void StartReceive()
    {
        bool pending = false;
        try
        {
            // 비동기 Receive
            pending = _socket.ReceiveAsync(_receiveArgs);
        }
        catch
        {
        }

        // 대기하지않고 바로 Receive가 되었다면 수행
        if (!pending)
        {
            OnReceiveCompleted(null, _receiveArgs);
        }
    }

    private void OnReceiveCompleted(object sender, SocketAsyncEventArgs e)
    {
        if (e.LastOperation == SocketAsyncOperation.Receive)
        {
            _messageResolver.OnReceive(e.Buffer, e.Offset, e.BytesTransferred, OnMessage);
        }
        else
        {
            _socket.Close();
        }

        StartReceive();
    }
      
    public void Send(string id, string message)
    {
        ChatMessage packet = new ChatMessage();
        packet.id = id;
        packet.message = message;
        Send(packet.ToByte(), packet.size);

        //// 보낼 메시지 바이트 배열
        //byte[] body = System.Text.Encoding.UTF8.GetBytes(message);
        //// 헤더부분 - 사이즈 2바이트
        //byte[] size = BitConverter.GetBytes((short)body.Length);
        //// 헤더부분 - 프로토콜아이디
        //byte[] protocolID = BitConverter.GetBytes((short)0); // 현재는 사용안하므로 0으로
        //// 패킷 (헤더+바디), CopyTo : 해당 배열을 다른 배열 특정위치에 복사
        //byte[] packet = new byte[body.Length + 4];
        //size.CopyTo(packet, 0);
        //protocolID.CopyTo(packet, 2);
        //body.CopyTo(packet, 4);

        //Send(packet, packet.Length);
        //_socket.Send(packet);
        //_socket.Send(System.Text.Encoding.UTF8.GetBytes(message));
    }

    public void Send(byte[] buffer, int length)
    {
        // 보내고 있지 않다면 리스트에 추가후 보내기 시작
        if (_sendList.Count == 0)
        {
            _sendList.Add(buffer);
            StartSend();
        }
        else
        {
            // 보내는 중이라면 리스트에만 추가, 현재 보내는것이 끝나면 보내게된다.
            _sendList.Add(buffer);
        }

        //_socket.Send(buffer, length, SocketFlags.None);
    }

    public void StartSend()
    {
        lock (_sendList)
        {
            // 리스트 처음요소를 가져와 버퍼에 복사후 보낸다. 
            byte[] buffer = _sendList[0];
            _sendArgs.SetBuffer(_sendArgs.Offset, buffer.Length);
            Array.Copy(buffer, 0, _sendArgs.Buffer, _sendArgs.Offset, buffer.Length);

            // 비동기 전송 시작.
            bool pending = _socket.SendAsync(_sendArgs);
            if (!pending)
            {
                OnSendCompleted(null, _sendArgs);
            }
        }
    }

    private void OnSendCompleted(object sender, SocketAsyncEventArgs e)
    {
        // 보낸 내용 삭제
        _sendList.RemoveAt(0);
        // 보낼것이 남아있다면 보낸다.
        if (_sendList.Count > 0)
        {
            StartSend();
        }
    }



    //public void ReceiveThread()
    //{
    //    try
    //    {
    //        while (true)
    //        {
    //            int length = _socket.Receive(_recvBuffer);
    //            if (length == 0)
    //            {
    //                break;
    //            }

    //            _messageResolver.OnReceive(_recvBuffer, 0, length, OnMessage);

    //            //SetMessage(System.Text.Encoding.UTF8.GetString(_recvBuffer, 0, length));
    //        }
    //    }
    //    catch (System.Exception e)
    //    {
    //        SetMessage("Receive Exception : " + e.Message);
    //    }
    //}

    private void OnMessage(byte[] message, int length)
    {
        ChatMessage packet = new ChatMessage();
        packet.ToPacket(message);
        SetMessage(packet.id + " : " + packet.message);

        // 헤더 부분을 제외하고 처리한다. (4바이트 이후의 것을 처리)
        //SetMessage(System.Text.Encoding.UTF8.GetString(message, MessageResolver.HEADER_SIZE, length - MessageResolver.HEADER_SIZE));
    }

    public void SetMessage(string message)
    {
        // 현재 열려진 폼에서 찾는다.
        foreach (Form form in Application.OpenForms)
        {
            if (form.Name == "FormClient")
            {
                FormClient formClient = form as FormClient;
                formClient.Message(message);
            }
        }
    }
}