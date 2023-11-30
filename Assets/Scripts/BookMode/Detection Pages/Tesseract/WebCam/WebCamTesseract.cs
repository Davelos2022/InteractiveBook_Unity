using OpenCvSharp;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public enum TypeWebCam
{
    BookMode,
    CalibrationMode
}

public class WebCamTesseract : MonoBehaviour
{
    [SerializeField] private TypeWebCam _type;
    [SerializeField] private WebCamSettings _webCamSettings;
    [Space]
    [Header("Output")]
    [SerializeField] private RawImage _leftPageCheck;
    [SerializeField] private RawImage _rightPageCheck;
    [SerializeField] private RawImage _fullOutputForCalibration;
    [Header("TextDetection settings")]
    [SerializeField] private float _secondWait;

    private TesseractScript _tesseract;
    private TesseractDriver _tesseractDriver;

    private WebCamTexture _webCamTexture; 
    private Mat _leftCheck;
    private Mat _rightCheck;
    private string _calibrationLeft; public string CalibrationLeft => _calibrationLeft;
    private string _calibrationRight; public string CalibrationRight => _calibrationRight;

    private void OnEnable()
    {
        Insalization();
    }

    private void OnDisable()
    {
        if (_webCamTexture != null && _webCamTexture.isPlaying)
            _webCamTexture.Stop();

        DisposeMat();
    }

    private void Update()
    {
        if (_webCamTexture != null)
            WebCamProccesing();
    }

    private void Insalization()
    {
        WebCamDevice[] devices = WebCamTexture.devices;

        if (devices.Length > 0)
        {
            _webCamTexture = new WebCamTexture(devices[0].name, _webCamSettings.Width, _webCamSettings.Height, _webCamSettings.FrameRate);
            _webCamTexture.Play();

            if (_type == TypeWebCam.CalibrationMode)
                StartCoroutine(Calibration());
            else
                StartCoroutine(DetectionPage());
        }
        else
        {
            _webCamSettings.NotCamError();
        }
    }

    private void DisposeMat()
    {
        _leftCheck.Dispose();
        _rightCheck.Dispose();
    }

    private void WebCamProccesing()
    {
        try
        {
            _leftCheck = OpenCvSharp.Unity.TextureToMat(_webCamTexture);
            _rightCheck = OpenCvSharp.Unity.TextureToMat(_webCamTexture);

            Mat cropLeftMat = CropImage(_leftCheck, (int)_webCamSettings.CropXLeftSlider.value, (int)_webCamSettings.CropYLeftSlider.value);
            Mat cropRighMat = CropImage(_rightCheck, (int)_webCamSettings.CropXRightSlider.value, (int)_webCamSettings.CropYRightSlider.value);

            Mat processMatLeft = ProcessImage(cropLeftMat);
            Mat processMatRight = ProcessImage(cropRighMat);


            if (_leftPageCheck.texture == null || _rightPageCheck.texture == null)
            {
                _leftPageCheck.texture = OpenCvSharp.Unity.MatToTexture(processMatLeft);
                _rightPageCheck.texture = OpenCvSharp.Unity.MatToTexture(processMatRight);
            }
            else
            {
                OpenCvSharp.Unity.MatToTexture(processMatLeft, (Texture2D)_leftPageCheck.texture);
                OpenCvSharp.Unity.MatToTexture(processMatRight, (Texture2D)_rightPageCheck.texture);
            }

        }
        catch
        {
            return;
        }

    }

    private Mat ProcessImage(Mat mat)
    {
        Mat grayImg = new Mat();
        Cv2.CvtColor(mat, grayImg, ColorConversionCodes.BGRA2GRAY);
        Cv2.GaussianBlur(grayImg, grayImg, new Size(5, 5), 6);
        Cv2.AdaptiveThreshold(grayImg, grayImg, 255, AdaptiveThresholdTypes.GaussianC, ThresholdTypes.BinaryInv, (int)_webCamSettings._thresholdSlider.value, (int)_webCamSettings._maskSlider.value);

        // Ќаходим контуры в обработанном изображении
        //Point[][] contours;
        //HierarchyIndex[] hierarchy;
        //Cv2.FindContours(grayImg, out contours, out hierarchy, RetrievalModes.External, ContourApproximationModes.ApproxSimple);

        // ‘ильтруем контуры, выбираем только те, площадь которых превышает пороговое значение
        //double areaThreshold = _webCamSettings._thresholdSlider.value;
        //var largeContours = contours.Where(c => Cv2.ContourArea(c) >= areaThreshold).ToList();

        //// —оздаем пустое изображение
        //Mat resultImage = Mat.Zeros(mat.Size(), MatType.CV_8UC3);

        //// «аполн€ем большие контуры белым цветом на пустом изображении
        //Cv2.DrawContours(resultImage, largeContours, (int)_webCamSettings._maskSlider.value, Scalar.White, (int)_webCamSettings._outMasSlider.value);

        return grayImg;
    }

    private Mat CropImage(Mat mat, int x, int y)
    {
        OpenCvSharp.Rect rect = new OpenCvSharp.Rect(x, y, _webCamSettings._widthCrop, _webCamSettings._heightCrop);
        Mat crop = new Mat(mat, rect);
        return crop;
    }

    private IEnumerator DetectionPage()
    {
        _tesseract = transform.GetComponent<TesseractScript>();

        while (_webCamTexture.isPlaying)
        {
            if (_leftPageCheck.texture != null && _rightPageCheck.texture != null)
            {
                _tesseract.DetectionText((Texture2D)_leftPageCheck.texture, TesseractScript.TypePage.Left);
                _tesseract.DetectionText((Texture2D)_rightPageCheck.texture, TesseractScript.TypePage.Right);
            }

            yield return new WaitForSeconds(_secondWait);
        }
    }

    private IEnumerator Calibration()
    {
        _fullOutputForCalibration.texture = _webCamTexture;
        _tesseractDriver = new TesseractDriver();
        _tesseractDriver.insalizationDriver();

        while (_webCamTexture.isPlaying)
        {
            if (_leftPageCheck.texture != null && _rightPageCheck.texture != null)
            {
                _calibrationLeft = _tesseractDriver.Recognize((Texture2D)_leftPageCheck.texture);
                _calibrationRight = _tesseractDriver.Recognize((Texture2D)_rightPageCheck.texture);
            }

            yield return new WaitForSeconds(_secondWait);
        }
    }
}