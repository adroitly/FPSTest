
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class GameClient
{
    //public static Hashtable allClient = new Hashtable();
    public static List<GameClient> allClient = new List<GameClient>();
    public static int frame;
    private static float curDt = 0;
    private static int perFrame = 15;
    private static int clientCount;
    public static List<string> cacheMsgs = new List<string>();
    public static List<string> allMsgs = new List<string>();
    public static List<string> ipList = new List<string>();
    private TcpClient _client;
    public string _clientIP;
    public string _clientNick;
    private byte[] data;
    public int uid;

    public GameClient(TcpClient client)
    {
        _client = client;
        _clientIP = client.Client.RemoteEndPoint.ToString();
        if (allClient.Count <= 200)
        {
            //allClient.Add(_clientIP, this);
            clientCount++;
            uid = allClient.Count;
            allClient.Add(this);
            ipList.Add(_clientIP);
            data = new byte[_client.ReceiveBufferSize];
            client.GetStream().BeginRead(data, 0, Convert.ToInt32(_client.ReceiveBufferSize), RceiveMessage, null);
            Loom.LogOnMainThread("登录成功");
            InitUserId();
            SendAllCacheMsgs();
        }
        else
        {
            //sendMessage("连接数量为最大，连接失败");
        }
    }

    public void SendAllCacheMsgs()
    {
        lock(allMsgs)
        {
            foreach(string msg in allMsgs)
            {
                SendMessage(msg);
            }
        }
    }

    public void InitUserId()
    {
        var cmd = new InitUserIdCmd();
        cmd.SetUserId(uid);
        SendMessage(cmd.Obj2Str());
        var temp = new CreateUserCmd();
        temp.SetUserId(uid);
        cacheMsgs.Add(temp.Obj2Str());
    }

    public void RceiveMessage(IAsyncResult ar)
    {
        int bytesread;
        try
        {
            lock (_client.GetStream())
            {
                bytesread = _client.GetStream().EndRead(ar);
            }
            if (bytesread < 1)
            {
                //allClient.Remove(_clientIP);
                allClient[uid] = null;
                Loom.LogOnMainThread("服务器错误");
                clientCount--;
                UserOut();
                return;
            }
            else
            {
                string messageReceived = Encoding.UTF8.GetString(data, 0, bytesread);
                Loom.LogOnMainThread("uid = " + uid + " Server Received" + messageReceived);
                lock (cacheMsgs)
                {
                    cacheMsgs.Add(messageReceived);
                }
                //sendMessage(messageReceived);
                lock (_client.GetStream())
                {
                    //Debug.Log("lock (this._client.GetStream()):");
                    _client.GetStream().BeginRead(data, 0, Convert.ToInt32(_client.ReceiveBufferSize),
                        RceiveMessage, null);
                }
            }
        }
        catch (Exception ex)
        {

            //Debug.Log("something wrong");
            //allClient.Remove(_clientIP);
            allClient[uid] = null;
            clientCount--;
            UserOut();
            Loom.LogOnMainThread(_clientNick + " leave");
        }
    }

    public void AddCacheMessage(string msg)
    {
        lock(cacheMsgs)
        {
            cacheMsgs.Add(msg);
        }
    }

    public void UserOut()
    {
        var cmd = new UserOutCmd();
        cmd.SetUserId(uid);
        AddCacheMessage(cmd.Obj2Str());
    }

    public static void SendAllMessage(string message)
    {
        allMsgs.Add(message);
        foreach(GameClient gameClient in allClient)
        {
            if (gameClient != null)
            {
                gameClient.SendMessage(message);
            }
        }
    }

    public static void CheckSendFrameMsgs()
    {
        if (clientCount == 0)
        {
            cacheMsgs.Clear();
            allMsgs.Clear();
        }
        curDt += Time.deltaTime;
        float perTime = 1.0f / perFrame;
        if (curDt >= perTime)
        {
            curDt -= perTime;
            if (clientCount > 1)
            {
                frame++;
                var cmd = new FrameStart();
                cmd.logFrame = frame;
                SendAllMessage(cmd.Obj2Str());
            }
            foreach (string msg in cacheMsgs)
            {
                SendAllMessage(msg);
            }
            cacheMsgs.Clear();
        }
    }

    public void SendServerMsg(string message)
    {
        SendMessage(message);
    }

    private void SendMessage(string message)
    {
        if (message == null)
            return;
        if (!message.EndsWith(CommonUtil.DataEndSplit))
        {
            message += CommonUtil.DataEndSplit;
        }
        byte[] sendData = Encoding.UTF8.GetBytes(message);

        //异步发送消息请求
        _client.Client.BeginSend(sendData, 0, sendData.Length, SocketFlags.None, new System.AsyncCallback(SendToServer), _client.Client);

    }
    //发送消息结束的回调函数
    void SendToServer(System.IAsyncResult ar)
    {
        Socket worker = ar.AsyncState as Socket;
        worker.EndSend(ar);
    }

}