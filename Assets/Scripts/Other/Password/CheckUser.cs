using UnityEngine;
using TMPro;
using Microsoft.Win32;
using VolumeBox.Toolbox.UIInformer;
using UnityEngine.UI;

public class CheckUser : MonoBehaviour
{
    [SerializeField] private GameObject[] _inputObject;
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _lebelText;
    [SerializeField] private ApplicationConfig _applicationConfig;
    public bool Authorization = false;

    public enum statusPassword { Autication, Exit};
    private statusPassword status;

    private string password;

    private string passwordApplication;
    
    public static CheckUser Instance;
    CheckUser()
    {
        Instance = this;
    }
    public void PasswordPanel(bool active)
    {
        transform.gameObject.SetActive(active);
    }

    public void SetPassword(string password)
    {
        passwordApplication = password;
    }

    public void InputButton(string input)
    {
        for (int x = 0; x < _inputObject.Length; x++)
        {
            if (!_inputObject[x].activeSelf)
            {
                _inputObject[x].SetActive(true);
                password += input;
                break;
            }
            else
            {
                continue;
            }
        }

        if (password.Length >= 4)
            CheckPassword();
    }

    public void CheckPassword()
    {
        if (password == passwordApplication  && status == statusPassword.Autication)
        {
            Authorization_panel();
            PasswordPanel(false);
        }
        else if (password == passwordApplication && status == statusPassword.Exit)
        {
            Application.Quit();
        }

        else
        {
            Error();
        }

    }

    public void ClearInput()
    {
        for (int x = 0; x < _inputObject.Length; x++)
            _inputObject[x].SetActive(false);

        password = "";
    }

    public void Clear()
    {
        if (password.Length > 0)
        {
            for (int x = _inputObject.Length - 1; x < _inputObject.Length; x--)
            {
                if (_inputObject[x].activeSelf)
                {
                    _inputObject[x].SetActive(false);
                    password = password.Remove(password.Length - 1);
                    break;
                }
            }
        }
    }

    private void Error()
    {
        Info.Instance.ShowBox("Вы ввели неверный пароль!", ClearInput, null, "Понятно");
    }

    private void OnEnable()
    {
        if (status == statusPassword.Autication)
        {
            _lebelText.text = "Авторизация";
            _titleText.text = "Для продолжение пожалуйста авторизуйтесь!";
        }

        if (status == statusPassword.Exit)
        {
            _lebelText.text = "Завершение сеанса";
            _titleText.text = "Для выхода из программы введите пароль";
        }


        var key = Registry.CurrentUser.OpenSubKey(_applicationConfig.RegistryKey);

        PlayerPrefs.SetString(_applicationConfig.RegistryValue, key?.GetValue(_applicationConfig.RegistryValue).ToString());
        Registry.CurrentUser.Close();

        ClearInput();
    }

    public void SetStatus(statusPassword statusPassword)
    {
        status = statusPassword;
    }

    public void Authorization_panel()
    {
        Authorization = !Authorization;
    }
}
