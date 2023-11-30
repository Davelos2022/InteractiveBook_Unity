using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewBooks : MonoBehaviour
{
    [SerializeField] private Image _leftPage;
    [SerializeField] private Image _rightPage;
    [SerializeField] private bool _showCover;

    private int _lastPage;
    private List<Sprite> _currentBook = new List<Sprite>();
    private List<int> _currentPages = new List<int>(2) { 0, 0 };

    private void OnEnable()
    {
        Insalization();
    }

    private void OnDisable()
    {
        ResetBook();
    }

    private void Insalization()
    {
        if (FileHandler._pdfpath != null)
            _currentBook = FileHandler.OpenPDF_file(_showCover);

        _lastPage = _currentBook.Count;
    }

    private void ResetBook()
    {
        _currentBook.Clear();

        _leftPage.sprite = null;
        _rightPage.sprite = null;

        DisablePage(TesseractScript.TypePage.Left);
        DisablePage(TesseractScript.TypePage.Right);
    }

    public void CheckNumberPage(int numberPage, TesseractScript.TypePage typePage)
    {
        if (_currentPages.Contains(numberPage) || numberPage > _currentBook.Count)
            return;

        if (numberPage == 0)
        {
            DisablePage(TesseractScript.TypePage.Left);
        }
        else if (numberPage >= _lastPage)
        {
            DisablePage(TesseractScript.TypePage.Left);
            DisablePage(TesseractScript.TypePage.Right);
            return;
        }

        switch (typePage)
        {
            case TesseractScript.TypePage.Left:
                _currentPages[0] = numberPage;
                ShowPage(_currentPages[0], _leftPage);
                break;
            case TesseractScript.TypePage.Right:
                _currentPages[1] = numberPage;
                ShowPage(_currentPages[1], _rightPage);
                break;
            default:
                break;
        }
    }

    private void ShowPage(int page, Image imagePage)
    {
        if (imagePage.sprite != _currentBook[_showCover ? page : page - 1])
            imagePage.gameObject.SetActive(false);

        imagePage.gameObject.SetActive(true);
        imagePage.sprite = _currentBook[_showCover ? page : page - 1];
        imagePage.preserveAspect = true;
    }

    public void DisablePage(TesseractScript.TypePage checkType)
    {
        if (checkType == TesseractScript.TypePage.Left && _leftPage.gameObject.activeSelf)
            _leftPage.gameObject.SetActive(false);

        if (checkType == TesseractScript.TypePage.Right && _rightPage.gameObject.activeSelf)
            _rightPage.gameObject.SetActive(false);
    }
}