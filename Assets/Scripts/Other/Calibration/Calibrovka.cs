using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class Calibrovka : MonoBehaviour
{
    [SerializeField] private WebCamTesseract _webCamDetection;
    [SerializeField] private WebCamDetectionQR _webCamDetectionQR;
    [SerializeField] private GameObject _completedLeft;
    [SerializeField] private GameObject _completedRight;

    public TypeDetection TypeDetection { get; set; }

    private bool _leftCheck;
    private bool _rightCheck;
    private string _resultLeft;
    private string _resultRight;

    private void OnDisable()
    {
        Clear();
    }

    private void Update()
    {
        Calibration();
    }

    private void Calibration()
    {
        if (TypeDetection == TypeDetection.TesseractMode)
        {
            if (!string.IsNullOrWhiteSpace(_webCamDetection.CalibrationLeft))
            {
                _resultLeft = TrimText(_webCamDetection.CalibrationLeft);
            }

            if (!string.IsNullOrWhiteSpace(_webCamDetection.CalibrationRight))
            {
                _resultRight = TrimText(_webCamDetection.CalibrationRight);
            }
            else
            {
                return;
            }
        }

        if (TypeDetection == TypeDetection.QRCodeMode)
        {

            if (!string.IsNullOrWhiteSpace(_webCamDetectionQR.CalibrationLeft))
            {
                _resultLeft = TrimText(_webCamDetectionQR.CalibrationLeft);
            }

            if (!string.IsNullOrWhiteSpace(_webCamDetectionQR.CalibrationRight))
            {
                _resultRight = TrimText(_webCamDetectionQR.CalibrationRight);
            }

            else 
            {
                return;
            }
        }


        _leftCheck = IsDigitsOnly(_resultLeft);
        _rightCheck = IsDigitsOnly(_resultRight);

        _completedLeft.SetActive(_leftCheck);
        _completedRight.SetActive(_rightCheck);

    }

    private string TrimText(string str)
    {
        return Regex.Replace(str, @"[^\d]", "");
    }

    private bool IsDigitsOnly(string str)
    {
        int resultNumber;
        return int.TryParse(str, out resultNumber);
    }

    private void Clear()
    {
        _leftCheck = false;
        _rightCheck = false;

        _resultLeft = null;
        _resultRight = null;

        _completedLeft.SetActive(_leftCheck);
        _completedRight.SetActive(_rightCheck);
    }
}