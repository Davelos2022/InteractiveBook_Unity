using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class Library : MonoBehaviour
{
    [SerializeField] private GameObject _bookPrefab;
    [SerializeField] private Transform _parrentBookLibary;
    [Space]
    [SerializeField] private GameObject _loadScreen;
    [SerializeField] private GameObject _bookMode;
    [Space]
    [SerializeField] private Button _addBookButton;
    [SerializeField] private Button _favoriteBookButton;
    [SerializeField] private Button _allBookButton;
    [Space]
    [SerializeField] private TMP_Dropdown dropdown;

    private List<GameObject> _books = new List<GameObject>();
    private List<GameObject> _bookFavorite = new List<GameObject>();

    public static Library Instance;
    public bool IsFavoriteMode = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _loadScreen.SetActive(false);
        _addBookButton.onClick.AddListener(ClickAdd_book);
        _favoriteBookButton.onClick.AddListener(ShowLikedBook);
        _allBookButton.onClick.AddListener(ShowAllBook);
        dropdown.onValueChanged.AddListener(OnSizeValueChanged);

        Refesh();
        SortAZ();
    }

    private void OnDestroy()
    {
        _addBookButton.onClick.RemoveListener(ClickAdd_book);
        _favoriteBookButton.onClick.RemoveListener(ShowLikedBook);
        _allBookButton.onClick.RemoveListener(ShowAllBook);
    }

    public void OnSizeValueChanged(int value)
    {
        if (value == 1)
            SortDate();
        if (value == 0)
            SortAZ();
    }

    private void SortAZ()
    {
        _books = _books.OrderBy(f => f.GetComponent<Book>().NameBook.text).ToList();

        for (int i = 0; i < _books.Count; i++)
        {
            _books[i].transform.SetSiblingIndex(i);
        }
    }

    private void SortDate()
    {
        _books = _books.OrderByDescending(f => f.GetComponent<Book>().dateTime.Ticks).ToList();

        for (int i = 0; i < _books.Count; i++)
        {
            _books[i].transform.SetSiblingIndex(i);
        }
    }
    public void Refesh(string[] path = null)
    {
        if (CheckUser.Instance.Authorization)
            _addBookButton.gameObject.SetActive(true);
        else
            _addBookButton.gameObject.SetActive(false);

        string[] pathFiles;

        if (path != null)
            pathFiles = path;
        else
            pathFiles = FileHandler.GetCountFiles();

        for (int x = 0; x < pathFiles.Length; x++)
        {
            GameObject bookObject = Instantiate(_bookPrefab, _parrentBookLibary);

            if (bookObject.GetComponent<Book>())
            {
                Book bookInPanel = bookObject.GetComponent<Book>();

                bookInPanel.ImageBook.sprite = FileHandler.LoadPDF(pathFiles[x], 0);
                bookInPanel.NameBook.text = Path.GetFileNameWithoutExtension(pathFiles[x]);
                bookInPanel.PathBook = pathFiles[x];
                bookInPanel.dateTime = FileHandler.DateCreation(pathFiles[x]);
                bookInPanel.ClickBTN.onClick.AddListener(() => ShowBook(bookInPanel));

                if (PlayerPrefs.HasKey($"{bookObject.GetComponent<Book>().NameBook.text}"))
                {
                    _bookFavorite.Add(bookObject);
                    bookInPanel.ActivePanel_Liked(true);
                }
            }

            _books.Add(bookObject);
        }
    }

    private void ClickAdd_book()
    {
        FileHandler.CheckDirectoory();

        string path = FileHandler.OpenFileDialog(true);

        if (path.Length > 0)
        {
            string nameBook = Path.GetFileNameWithoutExtension(path);

            FileHandler.CopyFile(path, nameBook);

            DestroyBook();
            Refesh();
        }
    }

    public void DeletedBook(Book bookInPanel)
    {
        if (_bookFavorite.Contains(bookInPanel.gameObject))
            _bookFavorite.Remove(bookInPanel.gameObject);

        _books.Remove(bookInPanel.gameObject);
    }

    public void DestroyBook()
    {
        if (_books.Count > 0)
        {
            for (int x = 0; x < _books.Count; x++)
                Destroy(_books[x]);

            _books.Clear();
        }

        if (_bookFavorite.Count > 0)
            _bookFavorite.Clear();
    }

    public void AddBook_Remove(Book bookInPanel)
    {
        if (_bookFavorite.Contains(bookInPanel.gameObject))
            _bookFavorite.Remove(bookInPanel.gameObject);
        else
            _bookFavorite.Add(bookInPanel.gameObject);
    }

    private void ShowLikedBook()
    {
        IsFavoriteMode = true;
        if (_books.Count > 0)
        {
            for (int x = 0; x < _books.Count; x++)
                _books[x].SetActive(false);
        }

        if (_bookFavorite.Count > 0)
        {
            for (int x = 0; x < _bookFavorite.Count; x++)
                _bookFavorite[x].SetActive(true);
        }
    }
    private void ShowAllBook()
    {
        IsFavoriteMode = false;

        if (_bookFavorite.Count > 0)
        {
            for (int x = 0; x < _bookFavorite.Count; x++)
                _bookFavorite[x].SetActive(false);
        }

        if (_books.Count > 0)
        {
            for (int x = 0; x < _books.Count; x++)
                _books[x].SetActive(true);
        }
    }

    private void ShowBook(Book book)
    {
        FileHandler._pdfpath = book.PathBook;
        StartCoroutine(LoadBook());
    }
    private IEnumerator LoadBook()
    {
        _loadScreen.SetActive(true);

        yield return new WaitForSeconds(0.7f);
        _loadScreen.SetActive(false);
        _bookMode.SetActive(true);
        this.gameObject.SetActive(false);
        yield break;
    }
}