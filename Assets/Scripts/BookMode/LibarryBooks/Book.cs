using UnityEngine;
using TMPro;
using UnityEngine.UI;
using VolumeBox.Toolbox.UIInformer;
using System;

public class Book : MonoBehaviour
{
    public Image ImageBook;
    public TextMeshProUGUI NameBook;
    public GameObject LikedSelection;
    [Space]
    public Button ClickBTN;
    public Button DeletedBTN;
    public Button LikedBTN;
    public Button ExportBTN;
    public DateTime dateTime;

    public string PathBook;

    private void OnEnable()
    {
        DeletedBTN.onClick.AddListener(ClickDeleted);
        LikedBTN.onClick.AddListener(ClickLiked);
        ExportBTN.onClick.AddListener(ExportBook);

        if (!CheckUser.Instance.Authorization)
            AdminMode(false);
        else
            AdminMode(true);  
    }

    private void OnDisable()
    {
        DeletedBTN.onClick.AddListener(ClickDeleted);
        LikedBTN.onClick.RemoveListener(ClickLiked);
        ExportBTN.onClick.RemoveListener(ExportBook);
    }

    private void ClickDeleted()
    {
        Info.Instance.ShowBox("Âû äåéñòâèòåëüíî õîòèòå \nóäàëèòü êíèãó?", DeletedPage, null, "ÓÄÀËÈÒÜ ÊÍÈÃÓ", "ÍÅ ÓÄÀËßÒÜ");
    }


    public void ExportBook()
    {
        string path = FileHandler.SaveFileDialog(null);

        if (path.Length > 0)
        {
            FileHandler.ExportInLibary(path, PathBook);
        }
    }

    private void DeletedPage()
    {
        if (PlayerPrefs.HasKey($"{NameBook.text}"))
            PlayerPrefs.DeleteKey($"{NameBook.text}");

        Library.Instance.DeletedBook(this);

        System.IO.File.Delete(PathBook);
        Destroy(this.gameObject);
    }

    private void ClickLiked()
    {
        if (PlayerPrefs.HasKey($"{NameBook.text}"))
            PlayerPrefs.DeleteKey($"{NameBook.text}");
        else
            PlayerPrefs.SetString($"{NameBook.text}", $"{NameBook.text}");

        Library.Instance.AddBook_Remove(this);
        ActivePanel_Liked(PlayerPrefs.HasKey($"{NameBook.text}"));

        if (!PlayerPrefs.HasKey($"{NameBook.text}") && Library.Instance.IsFavoriteMode)
            transform.gameObject.SetActive(false);
    }

    public void ActivePanel_Liked(bool active)
    {
        LikedSelection.SetActive(active);
    }

    private void AdminMode(bool admin)
    {
        DeletedBTN.gameObject.SetActive(admin);
        LikedBTN.gameObject.SetActive(admin);
        ExportBTN.gameObject.SetActive(admin);
    }
}