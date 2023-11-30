using BarcodeScanner;
using BarcodeScanner.Scanner;
using UnityEngine;
using UnityEngine.UI;
using OpenCvSharp;
using System.Text.RegularExpressions;
using System;

public enum TypeMode
{
    CalibrationMode,
    BookMode
}

public class WebCamDetectionQR : MonoBehaviour
{
    [SerializeField] private TypeMode _typeMode;
    [SerializeField] private ViewBooks _viewsBook;
    [SerializeField] private WebCamSettings _webCamCropSettings;
    [Space]
    [Header("Output")]
    [SerializeField] private RawImage _leftOutput;
    [SerializeField] private RawImage _rightOutput;
    [Header("QRDetection settings")]
    [SerializeField] private float _secondWait;

    private float RestartTime;
    private IScanner BarcodeScanner;

    private string _calibrationLeft; public string CalibrationLeft => _calibrationLeft;
    private string _calibrationRight; public string CalibrationRight => _calibrationRight;

    void Awake()
    {
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
    }

    private void OnEnable()
    {
        Insalization();
    }

    private void OnDisable()
    {
        Dispose();
    }

    void Update()
    {
        if (BarcodeScanner != null)
        {
            ProcessFrame(_leftOutput, (int)_webCamCropSettings.CropXLeftSlider.value, (int)_webCamCropSettings.CropYLeftSlider.value);
            ProcessFrame(_rightOutput, (int)_webCamCropSettings.CropXRightSlider.value, (int)_webCamCropSettings.CropYRightSlider.value);
            SetTextureDecorder();
            BarcodeScanner.Update();
        }

        if (RestartTime != 0 && RestartTime < Time.realtimeSinceStartup)
        {
            StartScanner();
            RestartTime = 0;
        }
    }

    private void Insalization()
    {
        BarcodeScanner = new Scanner();

        if (string.IsNullOrWhiteSpace(BarcodeScanner.Settings.WebcamDefaultDeviceName))
        {
            _webCamCropSettings.NotCamError();
            return;
        }

        BarcodeScanner.ResultQRCode += Result;
        BarcodeScanner.Camera.Play();

        BarcodeScanner.OnReady += (sender, arg) =>
        {
            SetupForCamera(_leftOutput);
            SetupForCamera(_rightOutput);

            RestartTime = Time.realtimeSinceStartup;
        };
    }

    private void Dispose()
    {
        BarcodeScanner.Camera.Stop();
        BarcodeScanner.Stop();
        BarcodeScanner.ResultQRCode -= Result;
        BarcodeScanner = null;
    }

    private void SetupForCamera(RawImage outputCamera)
    {
        BarcodeScanner.Camera.CropWidth = _webCamCropSettings._widthCrop;
        BarcodeScanner.Camera.CropHeight = _webCamCropSettings._heightCrop;
        outputCamera.texture = BarcodeScanner.Camera.Texture;
    }

    private void StartScanner()
    {
        BarcodeScanner.Scan((barCodeType, barCodeValue) =>
        {
            BarcodeScanner.Stop();
            RestartTime += Time.realtimeSinceStartup + _secondWait;
        });
    }

    private void ProcessFrame(RawImage output, int positionX, int positionY)
    {
        Mat frame = OpenCvSharp.Unity.TextureToMat(BarcodeScanner.Camera.WebCamTexture);
        Mat crop = CropImage(frame, (int)positionX, (int)positionY);
        output.texture = OpenCvSharp.Unity.MatToTexture(crop);
    }

    private Mat CropImage(Mat mat, int x, int y)
    {
        OpenCvSharp.Rect rect = new OpenCvSharp.Rect(x, y, _webCamCropSettings._widthCrop, _webCamCropSettings._heightCrop);
        Mat crop = new Mat();
        mat[rect].CopyTo(crop);
        return crop;
    }

    private void SetTextureDecorder()
    {
        BarcodeScanner.Camera.LeftOutput = (Texture2D)_leftOutput.texture;
        BarcodeScanner.Camera.RightOutput = (Texture2D)_rightOutput.texture;
    }

    string IsDigitsOnly(string str)
    {
        return Regex.Replace(str, @"[^\d]", "");
    }

    private void Result(string result, TypeResult typeResult)
    {
        try
        {
            int verifiedPage;
            result = IsDigitsOnly(result);
            verifiedPage = Convert.ToInt32(result);

            if (_typeMode == TypeMode.BookMode)
            {
                if (typeResult == TypeResult.Left)
                {
                    _viewsBook.CheckNumberPage(verifiedPage, TesseractScript.TypePage.Left);
                }
                else if (typeResult == TypeResult.Right)
                {
                    _viewsBook.CheckNumberPage(verifiedPage, TesseractScript.TypePage.Right);
                }
            }
            else if (_typeMode == TypeMode.CalibrationMode)
            {
                if (typeResult == TypeResult.Left)
                {
                    _calibrationLeft = result;
                }
                else if (typeResult == TypeResult.Right)
                {
                    _calibrationRight = result;
                }
            }
        }
        catch
        {
            return;
        }

        Debug.Log($"{result}");
    }
}
