using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System;

public class TcpClient 
{
    private void Log(string mgs)
    {
        //Debugger.Log(mgs);
    }
    Socket serverSocket; //服务器端socket
    IPAddress ip; //主机ip
    IPEndPoint ipEnd;
    string cacheMsg = "";
    string recvStr; //接收的字符串
    string ScanSuccessRecvStr;
    string sendStr; //发送的字符串
    byte[] recvData = new byte[1024]; //接收的数据，必须为字节
    byte[] ScanSuccessRecvData = new byte[1024];
    byte[] sendData = new byte[1024]; //发送的数据，必须为字节
    int recvLen = 0; //接收的数据长度
    Thread connectThread; //连接线程
    public Vector3 ServerVector;
    private Queue<string> queue = new Queue<string>();//新建一个队列，用来存储服务器发来的数据
    //服务器初始化
    public void InitSocket(string ipStr, int port)
    {
        ip = IPAddress.Parse(ipStr);
        ipEnd = new IPEndPoint(ip, port); //服务器端口号

        //开启一个线程连接
        connectThread = new Thread(new ThreadStart(SocketConnet));
        connectThread.Start();
    }

    //接收服务器消息
    void SocketConnet()
    {
        if (serverSocket != null)
            serverSocket.Close();
        //定义套接字类型,必须在子线程中定义
        serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //连接
        serverSocket.Connect(ipEnd);

        //recvStr = Encoding.Default.GetString(recvData,0,recvLen);
        //print(recvStr);
        //SocketReceive();
        this.serverSocket.BeginReceive(recvData, 0, recvData.Length, SocketFlags.None, new System.AsyncCallback(ReceiveFromServer), this.serverSocket);
    }

    private void ReceiveFromServer(System.IAsyncResult ar)
    {
        //获取当前正在工作的Socket对象
        Socket worker = ar.AsyncState as Socket;
        try
        {
            //接收完毕消息后的字节数
            recvLen = worker.EndReceive(ar);
        }
        catch (System.Exception ex)
        {
        }
        if (recvLen > 0)
        {
            string content = Encoding.Default.GetString(recvData, 0, recvLen);
            lock(content)
            {
                string[] datas = Split(cacheMsg + content, CommandUtil.DataEndSplit, out bool endIsSplit);
                for (int i = 0; i < datas.Length - 1; i ++)
                {
                    queue.Enqueue(datas[i]);
                    Log("recvStr" + datas[i]);
                }
                cacheMsg = datas[datas.Length - 1];
                if (endIsSplit)
                {
                    queue.Enqueue(cacheMsg);
                    Log("recvStr" + cacheMsg);
                    cacheMsg = "";
                }
                //queue.Enqueue(content);
            }
            //通过回调函数将消息返回给调用者
        }
        //继续异步等待接受服务器的返回消息
        worker.BeginReceive(recvData, 0, recvData.Length, SocketFlags.None, new System.AsyncCallback(ReceiveFromServer), this.serverSocket);
    }

    string []Split(string str, string split, out bool endIsSplit)
    {
        try
        {
            var temp = str;
            lock (str)
            {
                List<string> datas = new List<string>();
                StringBuilder sb = new StringBuilder();
                int len = split.Length;
                endIsSplit = str.EndsWith(split);
                for (; ; )
                {
                    if (temp.StartsWith(split))
                    {
                        temp = temp.Substring(len, temp.Length - len);
                        datas.Add(sb.ToString());
                        sb.Clear();
                    }
                    else
                    {
                        sb.Append(temp.Substring(0, 1));
                        temp = temp.Substring(1, temp.Length - 1);
                    }
                    if (temp.Length == 0)
                    {
                        break;
                    }
                }
                if (sb.Length != 0)
                {
                    datas.Add(sb.ToString());
                }
                if (datas.Count == 0)
                {
                    datas.Add(sb.ToString());
                }
                return datas.ToArray();
            }
        }catch(Exception e)
        {
            endIsSplit = false;
            return new string[0];
        }
        

    }
    public void SocketQuit()
    {
        //关闭线程
        if (connectThread != null)
        {
            connectThread.Interrupt();
            connectThread.Abort();
        }
        //最后关闭服务器
        if (serverSocket != null)
            serverSocket.Close();
    }

    public void SocketSend(string sendStr)
    {
        //清空发送缓存
        sendData = new byte[1024];
        //数据类型转换
        sendData = Encoding.UTF8.GetBytes(sendStr);
        //发送
        serverSocket.Send(sendData, sendData.Length, SocketFlags.None);
    }

    //返回接收到的字符串
    public string GetRecvStr()
    {
        string returnStr;
        //加锁防止字符串被改
        lock (this)
        {
            if (queue.Count > 0)
            {//队列内有数据时，取出
                returnStr = queue.Dequeue();
                return returnStr;
            }
        }
        return null;
    }

}