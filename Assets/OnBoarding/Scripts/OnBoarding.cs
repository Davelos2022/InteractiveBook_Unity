using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnBoarding : MonoBehaviour
{
    [SerializeField] private GameObject _selection;
    [SerializeField] private Transform[] _positionsBookMode;
    [SerializeField] private GameObject[] _panelsBookMode;
    [Space]
    [SerializeField] private GameObject _menuOne;
    [SerializeField] private GameObject _menuTwo;
    [SerializeField] private GameObject _libaryBookMode;
    [SerializeField] private GameObject _libaryBookMode_2;
    [SerializeField] private GameObject _editor;
    [SerializeField] private GameObject _bookMode;


    private float _speedMove = 1850f;
    private int _currentPosition;

    // Start is called before the first frame update
    void Start()
    {
        _currentPosition = 0;

        _panelsBookMode[_currentPosition].SetActive(true);
        _menuTwo.SetActive(true);

    }


    private IEnumerator MoveSeletion(Vector3 position)
    {
        while (_selection.transform.position != position)
        {
            _selection.transform.position = Vector3.MoveTowards(_selection.transform.position, position, _speedMove * Time.deltaTime);
            yield return null;
        }

        _panelsBookMode[_currentPosition].SetActive(true);
    }

    public void NextPosition()
    {
        _panelsBookMode[_currentPosition].SetActive(false);

        _currentPosition += 1;


        if (_currentPosition == 0 || _currentPosition == 6 || _currentPosition == 3 || _currentPosition == 4 || _currentPosition == 7 || _currentPosition == 5)
        {
            if (_currentPosition == 3)
                _libaryBookMode.SetActive(true);

            if (_currentPosition == 4)
                _bookMode.SetActive(true);

            if (_currentPosition == 5)
                _bookMode.SetActive(false);

            if (_currentPosition == 7)
                _libaryBookMode_2.SetActive(true);

            _panelsBookMode[_currentPosition].SetActive(true);
            _selection.transform.position = _positionsBookMode[_currentPosition].position;

        }
        else
        {
            StartCoroutine(MoveSeletion(_positionsBookMode[_currentPosition].position));
        }
    }

    public void BackPosition()
    {
        _panelsBookMode[_currentPosition].SetActive(false);

        _currentPosition -= 1;


        if (_currentPosition == 0 || _currentPosition == _positionsBookMode.Length - 2 || _currentPosition == 5 || _currentPosition == 2 || _currentPosition == 3 || _currentPosition == 6 || _currentPosition == 4)
        {
            if (_currentPosition == 2)
                _libaryBookMode.SetActive(false);

            if (_currentPosition == 3)
                _bookMode.SetActive(false);

            if (_currentPosition == 4)
                _bookMode.SetActive(true);

            if (_currentPosition == 6)
                _libaryBookMode_2.SetActive(false);

            _panelsBookMode[_currentPosition].SetActive(true);
            _selection.transform.position = _positionsBookMode[_currentPosition].position;

        }
        else
        {
            StartCoroutine(MoveSeletion(_positionsBookMode[_currentPosition].position));
        }
    }

    public void Completed_Skip(Button btn)
    {
        btn.interactable = false;

        PlayerPrefs.SetString("Tutorial_Completed", "Completed");

        UnityEngine.SceneManagement.SceneManager.LoadScene(2);
    }
}
