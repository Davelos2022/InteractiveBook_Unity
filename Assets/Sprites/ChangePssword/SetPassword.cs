using System.Collections;
using UnityEngine;
using TMPro;

public class SetPassword : MonoBehaviour
{
    [SerializeField] private GameObject[] _inputObject;
    [SerializeField] private GameObject[] _inputObjectTwoPanel;
    [SerializeField] private TextMeshProUGUI _textLebel;
    [SerializeField] private GameObject _panelOne;
    [SerializeField] private GameObject _panelTwo;
    [SerializeField] private GameObject _panelCompleted;

    private string passwordOne;
    private string passwordTwo;

    public void InputButtonOne(string input)
    {
        for (int x = 0; x < _inputObject.Length; x++)
        {
            if (!_inputObject[x].activeSelf)
            {
                _inputObject[x].SetActive(true);
                passwordOne += input;
                break;
            }
            else
            {
                continue;
            }
        }

        if (passwordOne.Length >= 4)
        {
            _panelTwo.SetActive(true);
            _panelOne.SetActive(false);
        }
    }

    public void InputButtonTwo(string input)
    {
        for (int x = 0; x < _inputObjectTwoPanel.Length; x++)
        {
            if (!_inputObjectTwoPanel[x].activeSelf)
            {
                _inputObjectTwoPanel[x].SetActive(true);
                passwordTwo += input;
                break;
            }
            else
            {
                continue;
            }
        }

        if (passwordTwo.Length >= 4)
            CheckPassword();
    }

    public void CheckPassword()
    {
        if (passwordOne == passwordTwo)
        {
            string passwordApp = passwordOne;
            PlayerPrefs.SetString("Password", passwordApp);
            _panelCompleted.SetActive(true);
            _panelTwo.SetActive(false);
        }
        else
        {
            Error();
        }
    }

    private void Error()
    {
        StartCoroutine(errorPassword());
    }

    public void ClearInputOne()
    {
        for (int x = 0; x < _inputObject.Length; x++)
            _inputObject[x].SetActive(false);

        passwordOne = "";
    }

    public void CleatInputtwo()
    {
        for (int x = 0; x < _inputObject.Length; x++)
            _inputObjectTwoPanel[x].SetActive(false);

        passwordTwo = "";
    }

    public void ClearOne()
    {
        if (passwordOne.Length > 0)
        {
            for (int x = _inputObject.Length - 1; x < _inputObject.Length; x--)
            {
                if (_inputObject[x].activeSelf)
                {
                    _inputObject[x].SetActive(false);
                    passwordOne = passwordOne.Remove(passwordOne.Length - 1);
                    break;
                }
            }
        }
    }

    public void ClearTwo()
    {
        if (passwordTwo.Length > 0)
        {
            for (int x = _inputObjectTwoPanel.Length - 1; x < _inputObjectTwoPanel.Length; x--)
            {
                if (_inputObjectTwoPanel[x].activeSelf)
                {
                    _inputObjectTwoPanel[x].SetActive(false);
                    passwordTwo = passwordTwo.Remove(passwordTwo.Length - 1);
                    break;
                }
            }
        }
    }

    private IEnumerator errorPassword()
    {
        ClearInputOne();
        CleatInputtwo();

        _panelOne.SetActive(true);
        _panelTwo.SetActive(false);

        string text = _textLebel.text;

        _textLebel.text = "Пароли не совпадают";

        yield return new WaitForSeconds(1.3f);

        _textLebel.text = text;

        yield break;
    }

    public void ExitApp()
    {
        Application.Quit();
    }

    public void BackPanel()
    {
        _panelOne.SetActive(true);
        _panelTwo.SetActive(false);
        _panelCompleted.SetActive(false);

        ClearInputOne();
        CleatInputtwo();
    }
}
