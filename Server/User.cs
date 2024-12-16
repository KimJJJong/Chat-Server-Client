using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

public class User
{
    private Socket _socket;
    private Thread _recvThread;
    private byte[] _recvBuffer = new byte[1024];
    private byte[] _sendBuffer = new byte[1024];
    private ChatServer _server;
    private MessageResolver _messageResolver;
    private SocketAsyncEventArgs _receiveArgs;
    private SocketAsyncEventArgs _sendArgs;
    private List<byte[]> _sendList = new List<byte[]>(); // Send할 바이트배열의 리스트

    public void Init(Socket socket, ChatServer server)
    {
        _socket = socket;
        _server = server;
        _messageResolver = new MessageResolver(2048);

        // 비동기 Receive
        _receiveArgs = new SocketAsyncEventArgs();
        _receiveArgs.SetBuffer(_recvBuffer, 0, _recvBuffer.Length);
        _receiveArgs.UserToken = socket; // 접속자에 대한 정보
        _receiveArgs.Completed += new System.EventHandler<SocketAsyncEventArgs>(OnReceiveCompleted);
        StartReceive();

        // 비동기 Send
        _sendArgs = new SocketAsyncEventArgs();
        _sendArgs.SetBuffer(_sendBuffer, 0, _sendBuffer.Length);
        _sendArgs.UserToken = socket;
        _sendArgs.Completed += new System.EventHandler<SocketAsyncEventArgs>(OnSendCompleted);

        //_recvThread = new Thread(ReceiveThread);
        //_recvThread.Start();
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

    //private void ReceiveThread()
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

    //            //_server.SetMessage(System.Text.Encoding.UTF8.GetString(_recvBuffer, 0, length));

    //            // 전체 에게 보낸다.
    //            //_server.SendAll(_recvBuffer, length);
    //        }
    //    }
    //    catch (System.Exception e)
    //    {
    //        _server.SetMessage("Receive Exception : " + e.Message);
    //    }

    //    // 유저가 나갔다면 삭제
    //    _server.RemoveUser(this);
    //}

    /// <summary>
    /// 받은 패킷을 처리
    /// </summary>
    /// <param name="message"></param>
    /// <param name="length"></param>
    private void OnMessage(byte[] message, int length)
    {
        ChatMessage packet = new ChatMessage();
        packet.ToPacket(message);
        _server.SetMessage(packet.id + " : " + packet.message);

        //// 헤더 부분을 제외하고 처리한다. (4바이트 이후의 것을 처리)
        //_server.SetMessage(System.Text.Encoding.UTF8.GetString(message, MessageResolver.HEADER_SIZE, length - MessageResolver.HEADER_SIZE));

        // 전체 에게 보낸다.
        _server.SendAll(message, length);
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

    public void Close()
    {
        _socket.Close();
    }
}