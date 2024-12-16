using Server;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

public class ChatServer
{
    private bool _run;
    private Socket _listenSocket;
    private List<User> _userList = new List<User>();    // 유저 관리 리스트
    SocketAsyncEventArgs _acceptArgs;   // 비동기 Accept를 위한 SocketAsyncEventArgs

    public void StartServer(int port)
    {
        if (_run)
        {
            return; 
        }

        try
        {
            _listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, port);
            _listenSocket.Bind(endPoint);
            _listenSocket.Listen(10);
        }
        catch (System.Exception e)
        {
            SetMessage("예외 발생 " + e.Message);
            return;
        }

        // 비동기 소켓 Accept
        _acceptArgs = new SocketAsyncEventArgs();
        _acceptArgs.Completed += new System.EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);
        StartAccept();

        // Accept 스레드
        //Thread acceptThread = new Thread(Accept);
        //acceptThread.Start();
        _run = true;

        SetMessage("서버 시작");
    }

    private void StartAccept()
    {
        // 재사용을 위해 null을 대입
        _acceptArgs.AcceptSocket = null;
        bool pending = false;
        try
        {
            // 비동기 Accept
            pending = _listenSocket.AcceptAsync(_acceptArgs);
        }
        catch
        {
        }

        // 대기하지않고 바로 Accept가 되었다면 수행
        if (!pending)
        {
            OnAcceptCompleted(null, _acceptArgs);
        }
    }

    private void OnAcceptCompleted(object sender, SocketAsyncEventArgs e)
    {
        if (e.SocketError == SocketError.Success)
        {
            // 유저 객체를 생성하고 리스트에 추가
            User user = new User();
            user.Init(e.AcceptSocket, this);
            lock (_userList)
            {
                _userList.Add(user);
            }
            SetMessage("유저 접속");
        }
        else
        {
            SetMessage("Accept 실패");
        }

        StartAccept();
    }

    public void EndServer()
    {
        if (!_run)
        {
            return;
        }

        _listenSocket.Close();
        foreach (User user in _userList)
        {
            user.Close();
        }
        _userList.Clear();
    }

    //private void Accept()
    //{
    //    try
    //    {
    //        while (true)
    //        {
    //            Socket sock = _listenSocket.Accept();

    //            // 유저 객체를 생성하고 리스트에 추가
    //            User user = new User();
    //            user.Init(sock, this);
    //            lock (_userList)
    //            {
    //                _userList.Add(user);
    //            }

    //            SetMessage("유저 접속");
    //        }
    //    }
    //    catch 
    //    {
    //    }
    //}

    public void SetMessage(string message)
    {
        // 현재 열려진 폼에서 찾는다.
        foreach (Form form in Application.OpenForms)
        {
            if (form.Name == "FormServer")
            {
                FormServer formServer = form as FormServer;
                formServer.Message(message);
            }
        }
    }

    public void SendAll(byte[] buffer, int length)
    {
        foreach (User user in _userList)
        {
            user.Send(buffer, length);
        }
    }

    public void RemoveUser(User user)
    {
        lock (_userList)
        {
            _userList.Remove(user);
        }
        SetMessage("유저 삭제");
    }
}