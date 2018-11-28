using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebCamera : MonoBehaviour
{

    public string DeviceName;
    public int webTextureWidth = 300;
    public int webTextureHeight = 300;
    public float CameraFPS;

    protected Texture2D mCachedTexture;

    //接收返回的图片数据  
    WebCamTexture _webCamera;
    public GUITexture _guiTexture;

    private void Start()
    {
        mCachedTexture = new Texture2D(webTextureWidth, webTextureHeight, TextureFormat.RGB24, false);
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(100, 100, 100, 100), "Initialize Camera"))
        {
            StartCoroutine(InitializeCamera());
        }

        //添加一个按钮来控制摄像机的开和关
        if (GUI.Button(new Rect(100, 250, 100, 100), "ON/OFF"))
        {
            if (null != _webCamera && null != _guiTexture)
            {
                if (_webCamera.isPlaying)
                    StopCamera();
                else
                    PlayCamera();
            }
        }

        if (GUI.Button(new Rect(100, 350, 100, 100), "Quit"))
        {
            Application.Quit();
        }

        if (GUI.Button(new Rect(100, 450, 100, 100), "Snap"))
        {
            getScreenShot();
        }

    }

    public void PlayCamera()
    {
        if (null != _guiTexture)
            _guiTexture.enabled = true;
        if (null != _webCamera)
            _webCamera.Play();
    }


    public void StopCamera()
    {
        if (null != _guiTexture)
            _guiTexture.enabled = false;
        if (null != _webCamera)
            _webCamera.Stop();
    }

    /// <summary>  
    /// 初始化摄像头
    /// </summary>  
    public IEnumerator InitializeCamera()
    {
        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);

        if (Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            WebCamDevice[] devices = WebCamTexture.devices;

            // Checks how many and which cameras are available on the device
            for (int i = 0; i < WebCamTexture.devices.Length; i++)
            {
                // We want the back camera
#if !UNITY_EDITOR
                if (!WebCamTexture.devices[i].isFrontFacing)
#endif
                {
                    //webCameraTexture = new WebCamTexture(cameraIndex, Screen.width, Screen.height);
                    _webCamera = new WebCamTexture(WebCamTexture.devices[i].name, webTextureWidth, webTextureHeight);

                    // Here we flip the GuiTexture by applying a localScale transformation
                    // works only in Landscape mode
                    _guiTexture.transform.localScale = new Vector3(1, 1, 1);

                    _guiTexture.texture = _webCamera;

                    _webCamera.Play();
                    break;
                }
            }

            //if (devices.Length > 0)
            //{
            //    DeviceName = devices[0].name;
            //    _webCamera = new WebCamTexture(DeviceName, (int)CameraSize.x, (int)CameraSize.y, (int)CameraFPS);

            //    if (null != _webRenderer)
            //    {
            //        _webRenderer.material.mainTexture = _webCamera;
            //        //_webRenderer.transform.localScale = Vector3.one;
            //    }

            //    _webCamera.Play();
            //}
        }
    }

    /// <summary>
	/// 连续捕获照片
	/// </summary>
	/// <returns>The photoes.</returns>
	public IEnumerator SeriousPhotoes()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();
            Texture2D t = new Texture2D(webTextureWidth, webTextureHeight, TextureFormat.RGB24, true);
            t.ReadPixels(new Rect(Screen.width / 2 - 180, Screen.height / 2 - 50, 360, 300), 0, 0, false);
            t.Apply();
            print(t);
            byte[] byt = t.EncodeToPNG();
            //			File.WriteAllBytes(Application.dataPath + "/MulPhotoes/" + Time.time.ToString().Split('.')[0] + "_" + Time.time.ToString().Split('.')[1] + ".png", byt);
            //Thread.Sleep(300);
        }
    }

    public void getScreenShot()
    {
        var begin = System.DateTime.Now.Ticks;
        mCachedTexture.ReadPixels(new Rect(Screen.width / 2 - webTextureWidth / 2, Screen.height / 2 - webTextureHeight / 2, webTextureWidth, webTextureHeight),0,0,false);
        mCachedTexture.Apply();
        //print(mCachedTexture);
        byte[] byt = mCachedTexture.EncodeToPNG();
        var dir = Application.dataPath + "/ScreenShot/";
        if(!System.IO.Directory.Exists(dir))
        {
            System.IO.Directory.CreateDirectory(dir);
        }
        var path = dir + Time.time.ToString().Split('.')[0] + "_" + Time.time.ToString().Split('.')[1] + ".png";
        System.IO.File.WriteAllBytes(path, byt);
        var end = System.DateTime.Now.Ticks;
        Debug.LogErrorFormat("get screen succeed ... path = <color=#00ff00>{0}</color> cost[<color=#00ff00>{1:F2}</color>]ms", path,(end - begin)*0.0001f);
    }
}