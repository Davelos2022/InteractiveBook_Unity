using UnityEngine;
using System;
using System.Linq;
using System.Text.RegularExpressions;

public class TesseractScript : MonoBehaviour
{
    [SerializeField] private ViewBooks Book;

    private TesseractDriver _tesseractDriver;
    private Texture2D _texture;

    public enum TypePage { Left, Right };
    private TypePage _typePage;



    private void Awake()
    {
        _tesseractDriver = new TesseractDriver();
        _tesseractDriver.insalizationDriver();
    }

    private void Recoginze(Texture2D outputTexture)
    {
        try
        {
            _texture = outputTexture;
            _tesseractDriver.Setup(OnSetupCompleteRecognize);
        }
        catch (Exception)
        {
            return;
        }
    }

    private void OnSetupCompleteRecognize()
    {
        OutputText(_tesseractDriver.Recognize(_texture));

    }

    private void OutputText(string text, bool isError = false)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            //Book.DisablePage(_pageCheck);
            return;
        }

        int verifiedPage;

        try
        {
            text = IsDigitsOnly(text);

            Debug.LogError(text);
            verifiedPage = Convert.ToInt32(text);
        }
        catch
        {
            return;
        }

        Book.CheckNumberPage(verifiedPage, _typePage);
    }

    string IsDigitsOnly(string str)
    {
        return Regex.Replace(str, @"[^\d]", "");
    }

    private string CutOff(string text)
    {
        string result = new string(text.Where(t => char.IsDigit(t)).ToArray());

        return result;
    }

    public void DetectionText(Texture2D texture2D, TypePage checkType)
    {
        if (texture2D == null) return;

        _typePage = checkType;

        texture2D.SetPixels32(texture2D.GetPixels32());
        texture2D.Apply();

        Recoginze(texture2D);
    }
}