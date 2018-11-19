using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebCamera : MonoBehaviour
{

    public string DeviceName;
    public Vector2 CameraSize;
    public float CameraFPS;

    //接收返回的图片数据  
    WebCamTexture _webCamera;
    public Renderer _webRenderer;//作为显示摄像头的面板


    void OnGUI()
    {
        if (GUI.Button(new Rect(100, 100, 100, 100), "Initialize Camera"))
        {
            StartCoroutine(InitializeCamera());
        }

        //添加一个按钮来控制摄像机的开和关
        if (GUI.Button(new Rect(100, 250, 100, 100), "ON/OFF"))
        {
            if (null != _webCamera && null != _webRenderer)
            {
                if (_webCamera.isPlaying)
                    StopCamera();
                else
                    PlayCamera();
            }
        }

        if (GUI.Button(new Rect(100, 450, 100, 100), "Quit"))
        {
            Application.Quit();
        }

    }

    public void PlayCamera()
    {
        if (null != _webRenderer)
            _webRenderer.enabled = true;
        if (null != _webCamera)
            _webCamera.Play();
    }


    public void StopCamera()
    {
        if (null != _webRenderer)
            _webRenderer.enabled = false;
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
            if(devices.Length > 0)
            {
                DeviceName = devices[0].name;
                _webCamera = new WebCamTexture(DeviceName, (int)CameraSize.x, (int)CameraSize.y, (int)CameraFPS);

                if (null != _webRenderer)
                {
                    _webRenderer.material.mainTexture = _webCamera;
                    //_webRenderer.transform.localScale = Vector3.one;
                }

                _webCamera.Play();
            }
        }
    }
}