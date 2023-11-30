using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuBookMode : MonoBehaviour
{
    [SerializeField] private Button _goToLibaryBTN;
    [SerializeField] private Button _infoBTN;
    [SerializeField] private Button _exitBTN;
    [SerializeField] private Button _tutorial;
    [SerializeField] private Button _calibrovkaBTN;
    [Space]
    [SerializeField] private GameObject _libary;
    [SerializeField] private GameObject _info;
    [SerializeField] private GameObject _menu;
    [SerializeField] private GameObject _calibrovka;

    private void Awake()
    {
        if (PlayerPrefs.HasKey("Password"))
        {
            string password = PlayerPrefs.GetString("Password");
            CheckUser.Instance.SetPassword(password);
        }
        else
        {
            CheckUser.Instance.SetPassword("1234");
        }
    }

    private void Start()
    {
        _goToLibaryBTN.onClick.AddListener(OpenLibrary);
        _infoBTN.onClick.AddListener(OpenInfo);
        _exitBTN.onClick.AddListener(ExitApp);
        _tutorial.onClick.AddListener(OpenTutorial);
        _calibrovkaBTN.onClick.AddListener(OpenCalibrovka);
    }

    private void OnDestroy()
    {
        _goToLibaryBTN.onClick.RemoveListener(OpenLibrary);
        _infoBTN.onClick.RemoveListener(OpenInfo);
        _exitBTN.onClick.RemoveListener(ExitApp);
        _tutorial.onClick.RemoveListener(OpenTutorial);
        _calibrovkaBTN.onClick.RemoveListener(OpenCalibrovka);
    }

    private void OpenLibrary()
    {
        _menu.SetActive(false);
        _libary.SetActive(true);
    }

    private void OpenInfo()
    {
        if (_info.activeSelf)
            _info.SetActive(false);

        _info.SetActive(true);
    }

    public void CloseInfo()
    {
        StartCoroutine(CloseInfoPanel());
    }

    private IEnumerator CloseInfoPanel()
    {
        _info.GetComponent<Animator>().SetTrigger("Exit");
        yield return new WaitForSeconds(1f);
        _info.SetActive(false);

        yield break;
    }

    private void OpenCalibrovka()
    {
        CheckUser.Instance.SetStatus(CheckUser.statusPassword.Autication);
        CheckUser.Instance.PasswordPanel(true);

        StartCoroutine(Authorization_Wait());
    }

    private void OpenTutorial()
    {
        _tutorial.interactable = false;

        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    private void ExitApp()
    {
        ExitApp_CallBack();
    }

    private void ExitApp_CallBack()
    {
        CheckUser.Instance.SetStatus(CheckUser.statusPassword.Exit);
        CheckUser.Instance.PasswordPanel(true);
    }

    private IEnumerator Authorization_Wait()
    {
        while (CheckUser.Instance.transform.gameObject.activeSelf)
            yield return null;

        if (CheckUser.Instance.Authorization)
            _calibrovka.SetActive(true);

        yield break;
    }
}