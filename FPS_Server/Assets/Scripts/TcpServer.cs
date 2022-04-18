using UnityEngine;
using System.Collections;
//using LitJson;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;
using System;

public class TcpServer : MonoBehaviour
{
    [SerializeField]
    private string ipStr = "127.0.0.1";
    [SerializeField]
    private int port = 9001;
    public static TcpServer Instance { get; private set; }
    TcpListener tcpListener;
    private void Start()
    {
        GameObject.DontDestroyOnLoad(this);
        Loom.RunAsync(() => {
            StartServer();
        });
    }
    private void Update()
    {
        GameClient.CheckSendFrameMsgs();
    }
    private void OnApplicationQuit()
    {
        tcpListener.Stop();
    }
    private void StartServer()
    {
        //const int bufferSize = 8792;//�����С,8192�ֽ�  

        IPAddress ip = IPAddress.Parse(ipStr);

        tcpListener = new TcpListener(ip, port);
        tcpListener.Start();

        //ִ��unity���̷߳�����GetComponent��Ҫ�����߳�����
        //Debug.Log("Socket��������������......");
        Loom.LogOnMainThread("  ������" + ip.ToString() + "��������......     " + DateTime.Now + "\n");

        do
        {
            TcpClient tcpClient = tcpListener.AcceptTcpClient();
            if (tcpClient != null && tcpClient.Connected)
            {
                var client = new GameClient(tcpClient);
                Loom.LogOnMainThread(client._clientIP + "   �ͻ�������     " + DateTime.Now + "\n");
                //client.SendUid();
            }
        }
        while (true);
    }

}