using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WebGLWebCamTest : MonoBehaviour
{
    private WebCamTexture cam_texture;
    private RenderTexture camera_renderer;

    private Rect _rect;
    private Texture2D _texture;
    private Vector2 _cameraSize = new Vector2(1920, 1080);
    private int _cameraFPS = 15;
    private WebCamTexture _webCameraTexture;
    public RawImage _rawImage;


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(nameof(GetWebCamPermission));   
    }

    IEnumerator GetWebCamPermission()
    {
        yield return new WaitForSeconds(10f);

        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
        if (Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            Debug.Log("webcam found");
            InitCameraDevice();
        }
        else
        {
            Debug.Log("webcam not found");
        }
    }


    //void startWebCam()
    //{
    //    WebCamDevice device = WebCamTexture.devices[0];

    //    cam_texture = new WebCamTexture(device.name);
    //    camera_renderer.tex = cam_texture;
    //    cam_texture.Play();
    //}

    public void InitCameraDevice()
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        _webCameraTexture = new WebCamTexture(devices[0].name, (int)_cameraSize.x, (int)_cameraSize.y, _cameraFPS);
        _rawImage.texture = _webCameraTexture;
        _webCameraTexture.Play();
    }

}
