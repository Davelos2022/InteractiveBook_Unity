using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LeftPanelMenu : MonoBehaviour
{
    [SerializeField] private Button _buttonAll;
    [SerializeField] private Button _buttonLiked;
    [SerializeField] private Button _buttonEditorMode;
    [Space]
    [SerializeField] private GameObject _activeButton_ALL;
    [SerializeField] private GameObject _activeButton_Liked;
    [Space]
    [SerializeField] private Image _imageBox;
    [SerializeField] private GameObject _editiorMode_Sprite;
    [SerializeField] private GameObject _readMode_Sprite;
    [SerializeField] private TextMeshProUGUI _txtButton;


    private void OnEnable()
    {
        _buttonAll.onClick.AddListener(Click_All);
        _buttonLiked.onClick.AddListener(Click_Liked);
        _buttonEditorMode.onClick.AddListener(Click_Editor);

        Click_All();
    }

    private void OnDisable()
    {
        _buttonAll.onClick.RemoveListener(Click_All);
        _buttonLiked.onClick.RemoveListener(Click_Liked);
        _buttonEditorMode.onClick.RemoveListener(Click_Editor);

        if (CheckUser.Instance.Authorization)
            CheckUser.Instance.Authorization_panel();

        Autification();
    }

    private void Click_All()
    {
        ActivePanel(false, _activeButton_Liked);

        ActivePanel(true, _activeButton_ALL);
    }

    private void Click_Liked()
    {
        ActivePanel(false, _activeButton_ALL);

        ActivePanel(true, _activeButton_Liked);
    }

    private void Click_Editor()
    {
        ActivePanel(false, _activeButton_ALL);
        ActivePanel(false, _activeButton_Liked);

        if (!CheckUser.Instance.Authorization)
        {
            CheckUser.Instance.SetStatus(CheckUser.statusPassword.Autication);
            CheckUser.Instance.PasswordPanel(true);
            StartCoroutine(Authorization_Wait());
        }
        else
        {
            CheckUser.Instance.Authorization_panel();
            Autification();
        }
    }

    private void ActivePanel(bool activator, GameObject panel)
    {
        panel.SetActive(activator);
    }

    private void Autification()
    {
        if (CheckUser.Instance.Authorization)
        {
            _readMode_Sprite.SetActive(false);
            _editiorMode_Sprite.SetActive(true);
            _txtButton.text = "Редактор";
        }
        else
        {
            _readMode_Sprite.SetActive(true);
            _editiorMode_Sprite.SetActive(false);
            _txtButton.text = "Читатель";
        }


        Library.Instance.DestroyBook();
        Library.Instance.Refesh();

        _activeButton_ALL.SetActive(true);
    }

    private IEnumerator Authorization_Wait()
    {
        while (CheckUser.Instance.transform.gameObject.activeSelf)
            yield return null;

        Autification();

        yield break;
    }
}