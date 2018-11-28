using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebCameraData
{
    public byte[] datas;
}

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
        mCachedTexture = new Texture2D(webTextureWidth, webTextureHeight, TextureFormat.RGBA32, false);
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
        }
    }

    public void getScreenShot()
    {
        var begin = System.DateTime.Now.Ticks;
        Debug.LogFormat("w={0} h={1}", _webCamera.width, _webCamera.height);
        mCachedTexture.SetPixels(_webCamera.GetPixels(_webCamera.width / 2 - webTextureWidth / 2, _webCamera.height / 2 - webTextureHeight / 2, webTextureWidth, webTextureHeight));
        var end = System.DateTime.Now.Ticks;
        Debug.LogErrorFormat("SetPixels32 cost[<color=#00ff00>{0:F2}</color>]ms", (end - begin) * 0.0001f);
        begin = System.DateTime.Now.Ticks;
        byte[] pngDatas = mCachedTexture.EncodeToPNG();
        var dir = Application.dataPath + "/ScreenShot/";
        if(!System.IO.Directory.Exists(dir))
        {
            System.IO.Directory.CreateDirectory(dir);
        }
        var path = dir + Time.time.ToString().Split('.')[0] + "_" + Time.time.ToString().Split('.')[1] + ".png";
        end = System.DateTime.Now.Ticks;
        Debug.LogErrorFormat("get screen succeed ... path = <color=#00ff00>{0}</color> cost[<color=#00ff00>{1:F2}</color>]ms length = <color=#00ff00>{2}</color> bytes", path, (end - begin) * 0.0001f, pngDatas.Length);

        begin = System.DateTime.Now.Ticks;
        var compressDatas = XZip.CompressZip(pngDatas);
        end = System.DateTime.Now.Ticks;

        Debug.LogErrorFormat("compressed length = <color=#00ff00>[{0}]:[{1:F2}]</color> bytes", compressDatas.Length, (end - begin) * 0.0001f);
        var uncompressDatas = XZip.DecompressZip(compressDatas);

        bool check = true;
        if(uncompressDatas.Length != pngDatas.Length)
        {
            check = false;
        }
        else
        {
            for (int i = 0; i < uncompressDatas.Length; ++i)
            {
                if (uncompressDatas[i] != pngDatas[i])
                {
                    check = false;
                    break;
                }
            }
        }
        if(check)
        {
            Debug.LogFormat("check succeed ...");
        }
        else
        {
            Debug.LogFormat("check failed ...");
        }
        //System.IO.File.WriteAllBytes(path, byt);
    }


}