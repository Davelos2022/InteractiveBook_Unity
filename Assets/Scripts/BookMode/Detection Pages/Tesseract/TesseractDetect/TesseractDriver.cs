using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

public class TesseractDriver
{
    private TesseractWrapper _tesseract;

    private string datapath = Path.Combine(Application.streamingAssetsPath, "tessdata");
    private bool isSetup;

    public void insalizationDriver()
    {
        _tesseract = new TesseractWrapper();
        isSetup = _tesseract.Init("eng", datapath);
    }

    public void Setup(UnityAction onSetupComplete)
    {
        if (onSetupComplete == null) return;

        OcrSetup(onSetupComplete);
    }

    public void OcrSetup(UnityAction onSetupComplete)
    {
        if (isSetup)
            onSetupComplete?.Invoke();
        else
            Debug.LogError(_tesseract.GetErrorMessage());
    }

    public string Recognize(Texture2D imageToRecognize)
    {
        return _tesseract.Recognize(imageToRecognize);
    }


}