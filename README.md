# BullyGang
# Unity Mobile Reference Game with Nearby Service

![image](https://user-images.githubusercontent.com/8115505/129139485-9ff8d0fe-49c0-4818-85ba-2cb4fe1876be.png)

The HMS Unity plugin helps you integrate all the power of Huawei Mobile Services in your Unity game:
 

# Game
 
![image](https://user-images.githubusercontent.com/8115505/129139564-b222f9e9-6af1-4ca7-a038-6ace8ce5f423.png)


This project is a basic hypercasual game for Unity mobile platform with Huawei Mobile Services Plugin Nearby Service integration.

Purpose: Try to not touch obstacle

**Nearby Service Features:**

Enables nearby devices to set up an Internet-free local network for direct data transmission.

Allows message publishing and subscription between nearby devices that run the app and are connected to the Internet.

Triggers scenario-specific interaction and services, and verifies the user presence using precise proximity sensing.

Outpaces the industry in speed of discovery and bandwidth with Huawei-developed protocols.

## Requirements
Android SDK min 23
Net 4.x

## Important
This plugin supports:
* Unity version 2019, 2020 - Developed in master Branch
* Unity version 2018 - Developed in 2.0-2018 Branch

**Make sure to download the corresponding unity package for the Unity version you are using from the release section**

## Troubleshooting 1
Please check our [wiki page](https://github.com/EvilMindDevs/hms-unity-plugin/wiki/Troubleshooting)

## Status
This is an ongoing project, currently WIP. Feel free to contact us if you'd like to collaborate and use Github issues for any problems you might encounter. We'd try to answer in no more than a working day.

## Connect your game Huawei Mobile Services in 5 easy steps

1. Register your app at Huawei Developer
2. Import the Plugin to your Unity project
3. Connect your game with the HMS Kit Managers

### 1 - Register your app at Huawei Developer

#### 1.1-  Register at [Huawei Developer](https://developer.huawei.com/consumer/en/)

#### 1.2 - Create an app in AppGallery Connect.
During this step, you will create an app in AppGallery Connect (AGC) of HUAWEI Developer. When creating the app, you will need to enter the app name, app category, default language, and signing certificate fingerprint. After the app has been created, you will be able to obtain the basic configurations for the app, for example, the app ID and the CPID.

1. Sign in to Huawei Developer and click **Console**.
2. Click the HUAWEI AppGallery card and access AppGallery Connect.
3. On the **AppGallery Connect** page, click **My apps**.
4. On the displayed **My apps** page, click **New**.
5. Enter the App name, select App category (Game), and select Default language as needed.
6. Upon successful app creation, the App information page will automatically display. There you can find the App ID and CPID that are assigned by the system to your app.

#### 1.3 Add Package Name
Set the package name of the created application on the AGC.

1. Open the previously created application in AGC application management and select the **Develop TAB** to pop up an entry to manually enter the package name and select **manually enter the package name**.
2. Fill in the application package name in the input box and click save.

> Your package name should end in .huawei in order to release in App Gallery

#### Generate a keystore.

Create a keystore using Unity or Android Tools. make sure your Unity project uses this keystore under the **Build Settings>PlayerSettings>Publishing settings**


#### Generate a signing certificate fingerprint.

During this step, you will need to export the SHA-256 fingerprint by using keytool provided by the JDK and signature file.

1. Open the command window or terminal and access the bin directory where the JDK is installed.
2. Run the keytool command in the bin directory to view the signature file and run the command.

    ``keytool -list -v -keystore D:\Android\WorkSpcae\HmsDemo\app\HmsDemo.jks``
3. Enter the password of the signature file keystore in the information area. The password is the password used to generate the signature file.
4. Obtain the SHA-256 fingerprint from the result. Save for next step.


#### Add fingerprint certificate to AppGallery Connect
During this step, you will configure the generated SHA-256 fingerprint in AppGallery Connect.

1. In AppGallery Connect, click the app that you have created and go to **Develop> Overview**
2. Go to the App information section and enter the SHA-256 fingerprint that you generated earlier.
3. Click √ to save the fingerprint.

____

### 2 - Import the plugin to your Unity Project

To import the plugin:

1. Download the [.unitypackage](https://github.com/EvilMindDevs/hms-unity-plugin/releases)
2. Open your game in Unity
3. Choose Assets> Import Package> Custom
![Import Package](http://evil-mind.com/huawei/images/importCustomPackage.png "Import package")
4. In the file explorer select the downloaded HMS Unity plugin. The Import Unity Package dialog box will appear, with all the items in the package pre-checked, ready to install.
![image](https://user-images.githubusercontent.com/6827857/113576269-e8e2ca00-9627-11eb-9948-e905be1078a4.png)
5. Select Import and Unity will deploy the Unity plugin into your Assets Folder
____

### 3 - Update your agconnect-services.json file.

In order for the plugin to work, some kits are in need of agconnect-json file. Please download your latest config file from AGC and import into Assets/StreamingAssets folder.
![image](https://user-images.githubusercontent.com/6827857/113585485-f488bd80-9634-11eb-8b1e-6d0b5e06ecf0.png)
____

### 4 - Connect your game with any HMS Kit

In order for the plugin to work, you need to select the needed kits Huawei > Kit Settings.

In this Nearby Bully Gang reference game , I selected the Nearby Service.

![image](https://user-images.githubusercontent.com/8115505/129139606-aa676142-6b41-4e7e-82cc-3ddbf02d1002.png)

Nearby Service is not dependent any other sevices.  
It will automaticaly create the GameObject for you and it has DontDestroyOnLoad implemented so you don't need to worry about reference being lost.

Now you need your game to call the Nearby Manager from your game. See below for further instructions.
    
Then you can call certain functions such as
```csharp
            Nearby.DiscoveryEngine.StartBroadcasting(mEndpointName, mFileServiceId, connectCallBack, advBuilder);

            Nearby.DiscoveryEngine.StopBroadcasting();
            
            ScanOption scanBuilder = new ScanOption.Builder().SetPolicy(Policy.POLICY_P2P).Build(); ;
            // Start scanning.
            Nearby.DiscoveryEngine.StartScan(mFileServiceId, scanEndpointCallback, scanBuilder);
 
```


## Troubleshooting 2
1.If you received package name error , please check your package name on File->Build Settings -> Player Settings -> Other Settings -> Identification

![image](https://user-images.githubusercontent.com/8115505/129139742-d15bad1e-2c98-4d33-b7e5-63838c27d516.png)

2.If you received min sdk error , 

![image](https://user-images.githubusercontent.com/67346749/125592730-940912c8-f9b4-4f8b-8fe4-b13532342613.PNG)

please set your API level as implied in the **Requirements** section

![image](https://user-images.githubusercontent.com/8115505/129139757-0ac507aa-459d-4d72-ab2f-4739afb76167.png)


 
### Nearby

Official Documentation on Nearby Kit: [Documentation](https://developer.huawei.com/consumer/en/doc/development/system-Guides/introduction-nearby-0000001060363166)
