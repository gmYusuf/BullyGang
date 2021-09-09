using HuaweiMobileServices.Base;
using HuaweiMobileServices.Nearby;
using HuaweiMobileServices.Nearby.Discovery;
using HuaweiMobileServices.Nearby.Transfer;
using System;
using UnityEngine;
using HuaweiMobileServices.Utils;

namespace HmsPlugin
{
    public class HMSNearbyServiceManager : HMSSingleton<HMSNearbyServiceManager>
    {
        private string scanInfo, remoteEndpointId = "Device2", transmittingMessage;
        private string myNameStr, mEndpointName = "Device", mFileServiceId = "com.bullygang.huawei";

        //Starting Broadcasting
        public void SendFilesInner(IConnectCallBack connectCallBack)
        {
            mEndpointName = SystemInfo.deviceName;
            
            Debug.Log("[HMS:] NearbyManager device name " + mEndpointName);

            Action mOnFailureListener;
            // Obtain the endpoint information.
            // Select a broadcast policy.
            BroadcastOption advBuilder = new BroadcastOption.Builder().SetPolicy(Policy.POLICY_P2P).Build();

            // Start broadcasting.
            ITask<HuaweiMobileServices.Utils.Void> StartBroadcastingTask = Nearby.DiscoveryEngine.StartBroadcasting(mEndpointName, mFileServiceId, connectCallBack, advBuilder);

            StartBroadcastingTask.AddOnSuccessListener((result) =>
            {
                Debug.Log("[HMS:] NearbyManager Success " + result);
                 
            }).AddOnFailureListener((exception) =>
            {
                Debug.Log("[HMS:] NearbyManager Failed " + exception.ErrorCode + " message " + exception.WrappedCauseMessage + " message2 " + exception.WrappedExceptionMessage);
                Debug.Log("[HMS:] NearbyManager Failed2 " + exception.ToString() + " message " + exception.Message + " message2 " + exception.StackTrace);
                Debug.Log("[HMS:] NearbyManager Failed3 " + exception.InnerException + " message " + exception.TargetSite + " message2 " + exception.Source);

            });
            Debug.Log("NearbyManager: Start Broadcasting");
        }

        //Starting Scanning
        public void OnScanResult(IScanEndpointCallback scanEndpointCallback)
        {
            myNameStr = SystemInfo.deviceName;

            Debug.Log("NearbyManager: device name " + myNameStr);
            ScanOption scanBuilder = new ScanOption.Builder().SetPolicy(Policy.POLICY_P2P).Build(); ;
            Debug.Log("NearbyManager: OnScanResult1 Start Scan" + mFileServiceId);
            // Start scanning.
            ITask<HuaweiMobileServices.Utils.Void> StartScan = Nearby.DiscoveryEngine.StartScan(mFileServiceId, scanEndpointCallback, scanBuilder);
            StartScan.AddOnSuccessListener((result) =>
            {
                Debug.Log("[HMS:] NearbyManager StartScan Success " + result);

            }).AddOnFailureListener((exception) =>
            {
                Debug.Log("[HMS:] NearbyManager Failed " + exception.ErrorCode + " message " + exception.WrappedCauseMessage + " message2 " + exception.WrappedExceptionMessage);
                Debug.Log("[HMS:] NearbyManager Failed2 " + exception.ToString() + " message " + exception.Message + " message2 " + exception.StackTrace);
                Debug.Log("[HMS:] NearbyManager Failed3 " + exception.InnerException + " message " + exception.TargetSite + " message2 " + exception.Source);

            });
            Debug.Log("NearbyManager: OnScanResult3 Start Scan");
        }

        //Stopping Broadcasting
        public void StopBroadCasting()
        {
            Nearby.DiscoveryEngine.StopBroadcasting();
        }

        //Stopping Scanning
        public void StopScanning()
        {
            // Nearby.GetDiscoveryEngine(context).StopScan();
            HuaweiMobileServices.Nearby.Message.GetOption reportPolicy = HuaweiMobileServices.Nearby.Message.GetOption.DEFAULT;
            Debug.Log("NearbyManager: Start reportPolicy workes" + reportPolicy);
        }
        public void InitiateConnection(string endpointId, ScanEndpointInfo scanEndpointInfo, IConnectCallBack connectCallBack)
        {
            //if (scanEndpointInfo.Name.Equals(scanInfo))
            {
                Debug.Log("[HMS] NearbyManager Client found Server and request connection. Server id:" + scanEndpointInfo.Name);
                // Initiate a connection.
                Nearby.DiscoveryEngine.RequestConnect(myNameStr, endpointId,   connectCallBack);
            }
        }

        // Confirming the Connection
        // Accept the connection request
        public void AcceptConnectionRequest(string endpointId, ConnectInfo connectInfo, IDataCallback dataCallback)
        {
            Debug.Log("[HMS] NearbyManager Accept Connection Request, Endpoint Name:" + connectInfo.EndpointName);
            Nearby.DiscoveryEngine.AcceptConnect(endpointId, dataCallback);
        }

        //Disconnecting from a Remote Endpoint
        public void DisconnectAllConnection()
        {
            Nearby.DiscoveryEngine.DisconnectAll();
        }

        //Transmitting Byte Arrays
        public void TransmittingBytes()
        {
            var message = System.Text.Encoding.UTF8.GetBytes(transmittingMessage);
            Nearby.TransferEngine.SendData(remoteEndpointId, Data.FromBytes(message));
        }
        //Transmitting Byte Arrays
        public void SendData(string remoteEndpointId, string transmittingData)
        {
            var message = System.Text.Encoding.UTF8.GetBytes(transmittingData);
            Nearby.TransferEngine.SendData(remoteEndpointId, Data.FromBytes(message));
        }
    }
}

