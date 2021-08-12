using HmsPlugin;
using HuaweiMobileServices.Nearby;
using HuaweiMobileServices.Nearby.Discovery;
using HuaweiMobileServices.Nearby.Transfer;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
 
public class GameManager :   HMSSingleton<GameManager>
{
    public GameObject player;
    // Next update in second
    private double nextUpdate = 0.5;

    private NearbyManagerListener nearbyManagerListener;
    public Action<string> OnDisconnected { get; set; }
    public Action<string, ConnectInfo> OnEstablish { get; set; }
    public Action<string, ConnectResult> OnResult { get; set; }
    public Action<string, ScanEndpointInfo> OnFound { get; set; }
    public Action<string> OnLost { get; set; }

    private string PlayerPositionX = "0";
    public bool PlayerSide = false;
    private bool IsConnected = false;
    private int IsServerOrClient = 0;

    private string serverEndPointID;
    private string clientEndPointID;


    public void BroadCasting()
    {
        HMSNearbyServiceManager.Instance.SendFilesInner(nearbyManagerListener);
        player = GameObject.Find("Deer L");
        PlayerSide = false;
    }
    public void DoStartScan()
    {
        HMSNearbyServiceManager.Instance.OnScanResult(nearbyManagerListener);
        player = GameObject.Find("Deer R");
        PlayerSide = true;

    }

    public void StopBroadCasting()
    {
        HMSNearbyServiceManager.Instance.StopBroadCasting();
    }
    public void StopScanning()
    {
        HMSNearbyServiceManager.Instance.StopScanning();
    }
    public void DisconnectAllConnection()
    {
        HMSNearbyServiceManager.Instance.DisconnectAllConnection();
    }
    public void AcceptConnectionRequest(string endpointId, ConnectInfo connectInfo)
    {
        NearbyManagerListener callBack = new NearbyManagerListener(this);
        HMSNearbyServiceManager.Instance.AcceptConnectionRequest(endpointId, connectInfo, callBack);
    }
    public void InitiateConnection(string endpointId, ScanEndpointInfo scanEndpointInfo)
    {

        NearbyManagerListener callBack = new NearbyManagerListener(this);
        HMSNearbyServiceManager.Instance.InitiateConnection(endpointId, scanEndpointInfo, callBack);
    }

    public class NearbyManagerListener : IConnectCallBack, IScanEndpointCallback, IDataCallback
    {
        private readonly GameManager nearbyManagerObject;
        private static string receivedMessage = "Receive Success";
        public NearbyManagerListener(GameManager nearbyManager)
        {
            nearbyManagerObject = nearbyManager;
        }
        //ConnectCallBack
        public void onDisconnected(string p0)
        {
            Debug.Log("[HMS] NearbyManager onDisconnected");
            nearbyManagerObject.OnDisconnected?.Invoke(p0);
            nearbyManagerObject.IsConnected = false;
        }

        public void onEstablish(string endpointId, ConnectInfo connectionInfo)
        {
            // Authenticating the Connection
            nearbyManagerObject.AcceptConnectionRequest(endpointId, connectionInfo);
            nearbyManagerObject.OnEstablish?.Invoke(endpointId, connectionInfo);
            Debug.Log("[HMS] NearbyManager onEstablish. Client accept connection from " + endpointId);

        }

        public void onResult(string endpointId, ConnectResult resultObject)
        {
            Debug.Log("[HMS] NearbyManager onResult");

            if (resultObject.Status.StatusCode == StatusCode.STATUS_SUCCESS)
            {
                /* The connection was established successfully, we can exchange data. */
                Debug.Log("[HMS] NearbyManager Connection Established. STATUS SUCCESS");
                HMSNearbyServiceManager.Instance.StopScanning();
                nearbyManagerObject.ServerConnected();
            }
            else if (resultObject.Status.StatusCode == StatusCode.STATUS_CONNECT_REJECTED)
            {
                Debug.Log("[HMS] NearbyManager Connection Established. STATUS REJECTED");
            }
            else
            {
                Debug.Log("[HMS] NearbyManager Connection Established.  status code" + resultObject.Status.StatusCode);
            }
            nearbyManagerObject.OnResult?.Invoke(endpointId, resultObject);
            nearbyManagerObject.clientEndPointID = endpointId;
            nearbyManagerObject.IsServerOrClient = -1; // server

        }

        //ScanEndpointCallback
        public void onFound(string endpointId, ScanEndpointInfo discoveryEndpointInfo)
        {
            nearbyManagerObject.serverEndPointID = endpointId;
            nearbyManagerObject.IsServerOrClient = 1; // client
            Debug.Log("[HMS] NearbyManager OnFound");
            nearbyManagerObject.InitiateConnection(endpointId, discoveryEndpointInfo);
            nearbyManagerObject.OnFound?.Invoke(endpointId, discoveryEndpointInfo);
        }

        public void onLost(string endpointId)
        {
            Debug.Log("[HMS] NearbyManager OnLost");
            nearbyManagerObject.OnLost?.Invoke(endpointId);
        }

        // DataCallback
        public void onReceived(string endpointId, Data dataReceived)
        {
            if (dataReceived.DataType == Data.Type.BYTES)
            {
                string msg = System.Text.Encoding.UTF8.GetString(dataReceived.AsBytes);
                if (msg.Equals(receivedMessage))
                {
                    Debug.Log("[HMS] NearbyManager Received ACK. Send next.");
                }
            }
        }
        public void onTransferUpdate(string endpointId, TransferStateUpdate update)
        {
            Debug.Log("[HMS] NearbyManager onTransferUpdate");
        }
    }

    private void ServerConnected()
    {
        //HMSNearbyServiceManager.Instance.SendData(PlayerPositionX);
        Debug.Log("[HMS] NearbyManager Server Connected Send Position " + PlayerPositionX);
        GameObject broadCastButton = GameObject.Find("Server");
        GameObject scanButton = GameObject.Find("Connect");
        broadCastButton.SetActive(false);
        scanButton.SetActive(false);
        IsConnected = true;
        Debug.Log("[HMS] NearbyManager Button Hide " );
 
    }

    void Update()
    {
        // If the next update is reached
        if (Time.time >= nextUpdate && IsConnected)
        {
            Debug.Log(Time.time + ">=" + nextUpdate);
            // Change the next update (current second+1)
            nextUpdate = Mathf.FloorToInt(Time.time) + 0.5;
            PlayerPositionX = player.transform.position.x.ToString("R");
            // Call your fonction
            UpdateEverySecond();
        }
    }
    // Update is called once per second
    void UpdateEverySecond()
    {
        if(IsServerOrClient == -1)
            HMSNearbyServiceManager.Instance.SendData(clientEndPointID, PlayerPositionX);
        else if(IsServerOrClient == 1)
            HMSNearbyServiceManager.Instance.SendData(serverEndPointID,  PlayerPositionX);

    }

    private void Start()
    {
        nearbyManagerListener = new NearbyManagerListener(this);
    }
}
