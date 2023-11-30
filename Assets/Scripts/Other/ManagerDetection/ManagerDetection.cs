using UnityEngine;

public enum TypeDetection
{ 
   TesseractMode,
    QRCodeMode
}


public class ManagerDetection : MonoBehaviour
{
    [SerializeField] private TypeDetection _typeDetection;
    [SerializeField] private GameObject _bookMode;
    [SerializeField] private GameObject _calibration;
    [Space]
    [SerializeField] private WebCamSettings _webCamSettings;
    [SerializeField] private Calibrovka _calibrationScripts;
    void Awake()
    {
        Insalization();
    }

    private void Insalization()
    {
        switch (_typeDetection)
        {
            case TypeDetection.QRCodeMode:
                TesseractDetection(false);
                QRCodeDetection(true);
                break;
            case TypeDetection.TesseractMode:
                TesseractDetection(true);
                QRCodeDetection(false);
                break;
        }

        _webCamSettings.TypeDetection = _typeDetection;
        _calibrationScripts.TypeDetection = _typeDetection;
    }

    private void TesseractDetection(bool activator)
    {
        _bookMode.GetComponent<WebCamTesseract>().enabled = activator;
        _calibration.GetComponent<WebCamTesseract>().enabled = activator;
    }

    private void QRCodeDetection(bool activator)
    {
        _bookMode.GetComponent<WebCamDetectionQR>().enabled = activator;
        _calibration.GetComponent<WebCamDetectionQR>().enabled = activator;
    }
}
