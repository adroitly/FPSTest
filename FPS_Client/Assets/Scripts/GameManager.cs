using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static TcpClient TcpClient { get; private set; }
    public static GameManager Instance
    {
        get;
        private set;
    }

    [SerializeField]
    private string ipStr;
    [SerializeField]
    private int port;
    private bool isInit = false;

    private void Awake()
    {
        Instance = this;
        FPSMain.Instance.InitFPS();
        TcpClient = new TcpClient();
        TcpClient.InitSocket(ipStr, port);
        Application.targetFrameRate = 60;
    }

    private void OnApplicationQuit()
    {
        TcpClient.SocketQuit();
    }

    private void Update()
    {
        if(!isInit && FPSMain.Instance.userId > -1)
        {
            isInit = true;
        }
        if (isInit && Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(ray, out hit))
            {
                var pos = hit.point;
                var cmd = new MoveToCmd()
                {
                    targetPosition = pos.ToVec3(),
                };
                FPSMain.Instance.SendCommand(cmd);
            }

        }
        FPSMain.Instance.DoActions();
    }
}
