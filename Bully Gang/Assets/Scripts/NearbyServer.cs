using HmsPlugin;
using HuaweiMobileServices.Nearby;
using HuaweiMobileServices.Nearby.Discovery;
using HuaweiMobileServices.Nearby.Transfer;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class NearbyServer : HMSSingleton<NearbyServer>
{
    public Action<string, ConnectInfo> OnEstablish { get; set; }
    public GameObject player;
    // Next update in second
    private double nextUpdate = 0.5;
    public NearbyManagerConnectListener nearbyManagerListener;
    private bool IsConnected = false;
    private string clientEndPointID;
    public Action<string, ConnectResult> OnResult { get; set; }
    private int IsServerOrClient = 0;
    public Action<string> OnDisconnected { get; set; }
    public string PlayerPositionX { get => playerPositionX; set => playerPositionX = value; }
    public string ClientPosition { get => clientPosition; set => clientPosition = value; }

    private string playerPositionX = "0";
    private string clientPosition = "no";
    private NearbyManagerDataListener dataCallBack;


    private string serverEndPointID;

    // Start is called before the first frame update
    void Start()
    {
        nearbyManagerListener = new NearbyManagerConnectListener(this);

    }

    void Update()
    {
        // If the next update is reached
        if (Time.time >= nextUpdate && IsConnected)
        {
            //Debug.Log(Time.time + ">=" + nextUpdate);
            // Change the next update (current second+1)
            nextUpdate = Mathf.FloorToInt(Time.time) + 0.5;
            PlayerPositionX =   CrossPlatformInputManager.GetAxis("Horizontal").ToString();     
            // Call your fonction
            UpdateEverySecond();
        }
    }
    // Update is called once per second
    void UpdateEverySecond()
    {
        if (IsServerOrClient == -1)
            HMSNearbyServiceManager.Instance.SendData(clientEndPointID, PlayerPositionX);
        else if (IsServerOrClient == 1)
            HMSNearbyServiceManager.Instance.SendData(serverEndPointID, PlayerPositionX);

    }
    public void AcceptConnectionRequest(string endpointId, ConnectInfo connectInfo)
    {
        dataCallBack = new NearbyManagerDataListener(this);
        HMSNearbyServiceManager.Instance.AcceptConnectionRequest(endpointId, connectInfo, dataCallBack);
    }
    public class NearbyManagerConnectListener : IConnectCallBack
    {
        private readonly NearbyServer nearbyManagerObject;
        private static string receivedMessage = "Receive Success";
        public NearbyManagerConnectListener(NearbyServer nearbyManager)
        {
            Debug.Log("[HMS] NearbyManager NearbyManagerListener");

            nearbyManagerObject = nearbyManager;
        }

        public void onEstablish(string endpointId, ConnectInfo connectionInfo)
        {
            Debug.Log("[HMS] NearbyManager onEstablish");
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

        public void onDisconnected(string p0)
        {
            Debug.Log("[HMS] NearbyManager onDisconnected");
            nearbyManagerObject.OnDisconnected?.Invoke(p0);
            nearbyManagerObject.IsConnected = false;
        }
    }

    public class NearbyManagerDataListener : IDataCallback
    {
        private readonly NearbyServer nearbyManagerObject;
        private static string receivedMessage = "Receive Success";

        public NearbyManagerDataListener(NearbyServer nearbyManager)
        {
            Debug.Log("[HMS] NearbyManager NearbyManagerListener");

            nearbyManagerObject = nearbyManager;
        }

        public void onReceived(string endpointId, Data dataReceived)
        {
            if (dataReceived.DataType == Data.Type.BYTES)
            {
                string msg = System.Text.Encoding.UTF8.GetString(dataReceived.AsBytes);
                if (msg.Equals(receivedMessage))
                {
                    Debug.Log("[HMS] NearbyManager Received ACK. Send next.");
                }
                nearbyManagerObject.ClientPosition = msg;

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
        Debug.Log("[HMS] NearbyManager Button Hide ");

    }
}
