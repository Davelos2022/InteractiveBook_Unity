using System.IO;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using Paroxe.PdfRenderer;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Cysharp.Threading.Tasks;
using VolumeBox.Toolbox.UIInformer;

public static class FileHandler
{
    private static string _bookPath = Application.streamingAssetsPath + "/Books/";
    public static string _pdfpath = null;

    public static List<Sprite> _booksPages = new List<Sprite>();

    public static bool editMode = false;

    public static Sprite coverBook;

    public static void AddPage_InBook(Sprite page, int index)
    {
        if (index > _booksPages.Count - 1 || index == 0 && _booksPages.Count <= 0)
            _booksPages.Add(page);
        else
            _booksPages[index] = page;
    }

    public static string[] GetCountFiles()
    {
        var checkFormats = new[] { ".pdf" };

        var countFiles = Directory
            .GetFiles(_bookPath)
            .Where(file => checkFormats.Any(file.ToLower().EndsWith))
            .ToArray();

        return countFiles;
    }

    public static DateTime DateCreation(string path)
    {
        var info = new FileInfo(path);
        //var xx = info.GetFiles().Where(x => x.Extension.ToLower() == ".pdf").OrderByDescending(f => f.CreationTime).ToList().ConvertAll(x => x.FullName).ToArray();

        var dateFile = info.CreationTime;

        return dateFile;
    }

    public static List<Sprite> OpenPDF_file(bool cover = false)
    {
        PDFDocument pdfDocument = new PDFDocument(_pdfpath, "");
        List<Sprite> pdfPages = new List<Sprite>();

        int countPage = pdfDocument.GetPageCount();

        for (int x = 0; x < countPage; x++)
            pdfPages.Add(LoadPDF(_pdfpath, x));

        if (!cover)
            pdfPages.RemoveAt(0);

        return pdfPages;
    }

    public static void CheckDirectoory()
    {
        if (Directory.Exists(_bookPath))
            return;
        else
            Directory.CreateDirectory(_bookPath);
    }

    public static Sprite LoadImage(string imagePath)
    {
        if (string.IsNullOrEmpty(imagePath) || string.IsNullOrWhiteSpace(imagePath)) return null;

        var relativePath = imagePath;

        var fileContent = File.ReadAllBytes($"{relativePath}");

        Texture2D tex = new Texture2D(1, 1);
        tex.LoadImage(fileContent);


        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(tex.width / 2, tex.height / 2));
    }

    public static Sprite LoadPDF(string pdfPath, int pageNumber = 0)
    {
        PDFDocument pdfDocument = new PDFDocument(pdfPath, "");

        if (pdfDocument.IsValid)
        {
            PDFRenderer renderer = new PDFRenderer();
            Texture2D texture = renderer.RenderPageToTexture(pdfDocument.GetPage(pageNumber));

            texture.filterMode = FilterMode.Bilinear;
            texture.anisoLevel = 8;

            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(texture.width, texture.height));
        }
        else
        {
            return null;
        }
    }

    public static async UniTask<string> SaveImageToFile(Texture2D pageTexture, string bookName, int page, byte[] texByteArray)
    {
        if (pageTexture == null) return "";

        if (!Directory.Exists(_bookPath + bookName))
            Directory.CreateDirectory(_bookPath + bookName);

        var fileName = $"{_bookPath}{bookName}" + "/" + $"page_{page}";

        FileStream fileStream = null;

        try
        {
            fileStream = File.Open($"{fileName}.png", FileMode.OpenOrCreate);
        }
        catch
        {
            Debug.Log($"{fileName}.png save failed");
        }

        if (fileStream == null) return fileName;

        var task = UniTask.RunOnThreadPool(() => fileStream.WriteAsync(texByteArray, 0, texByteArray.Length));
        await task;
        fileStream.Dispose();
        fileStream.Close();
        return fileName;
    }

    public static bool CheckBook(string nameBook)
    {
        string path = _bookPath + nameBook + ".pdf";

        if (File.Exists(path))
            return true;
        else
            return false;
    }

    public static bool CheckExportBook(string pathBook)
    {
        string path = pathBook + ".pdf";

        if (File.Exists(path))
            return true;
        else
            return false;

    }

    public async static UniTask<bool> SaveBookIn_PDF(string path, string nameBook, Sprite cover, GameObject size, bool open = false)
    {
        try
        {
            if (path == null)
            {
                path = _bookPath + nameBook + ".pdf";

                if (File.Exists(path))
                    File.Delete(path);
            }

            if (_booksPages[0] != coverBook)
                _booksPages.Insert(0, coverBook);

            //Create recovery Image
            for (int x = 0; x < _booksPages.Count; x++)
            {
                var tex = _booksPages[x].texture;
                var texBytes = tex.EncodeToPNG();
                await UniTask.RunOnThreadPool(() => SaveImageToFile(tex, nameBook, x, texBytes));
            }

            //Save PDF
            RectTransform sizeImage = size.GetComponent<RectTransform>();

            Rectangle sizePage = new Rectangle(sizeImage.rect.width + 100, sizeImage.rect.height + 100);
            var doc = new Document(sizePage);
            PdfWriter.GetInstance(doc, new FileStream($"{path}", FileMode.Create));

            //PDF proccesing
            doc.Open();

            var info = new DirectoryInfo(_bookPath + nameBook);
            var files = info.GetFiles().Where(x => x.Extension.ToLower() == ".png").OrderBy(f => f.CreationTime).ToList().ConvertAll(x => x.FullName).ToArray();


            //for (int x = 0; x < files.Length; x++)
            //    Debug.Log(files[x]);

            foreach (var pathInDirectory in files)
            {
                await UniTask.RunOnThreadPool(() =>
                {
                    var image = iTextSharp.text.Image.GetInstance(pathInDirectory);
                    image.Alignment = Element.ALIGN_CENTER;
                    doc.Add(image);
                });
            }

            doc.Close();

            //Clear recovery Iamege

            if (Directory.Exists(_bookPath + nameBook))
            {
                DirectoryInfo di = new DirectoryInfo(_bookPath + nameBook);

                foreach (FileInfo file in di.GetFiles())
                {
                    await UniTask.RunOnThreadPool(() => file.Delete());
                }


                Directory.Delete(_bookPath + nameBook);
            }

            //Copy file and Open 
            //CopyFile(path, nameBook);

            if (open)
                Application.OpenURL(path);

            return true;
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Failed to save book: {e.Message}");
            return false;
        }
    }


    public static void CopyFile(string path, string name)
    {
        if (File.Exists(_bookPath + $"{name}.pdf"))
            File.Delete(_bookPath + $"{name}.pdf");

        File.Copy(path, _bookPath + $"{name}.pdf");
    }

    public static void ExportInLibary(string path, string pathOut)
    {
        if (File.Exists(_bookPath + $"{pathOut}.pdf"))
            File.Delete(_bookPath + $"{pathOut}.pdf");

        File.Copy($"{pathOut}", path);

    }

    public static void DeletedBook(string nameBook)
    {
        if (Directory.Exists(_bookPath + nameBook))
            Directory.Delete(_bookPath + nameBook);
        else
            return;
    }

    public static string OpenFileDialog(bool pdf = false)
    {
        OpenFileName ofn = new OpenFileName();
        ofn.structSize = Marshal.SizeOf(ofn);

        if (pdf)
            ofn.filter = "PDF Files\0*.pdf;\0\0";
        else
            ofn.filter = "Image Files\0*.jpg;*.png\0\0";

        ofn.file = new string(new char[256]);
        ofn.maxFile = ofn.file.Length;
        ofn.fileTitle = new string(new char[64]);
        ofn.maxFileTitle = ofn.fileTitle.Length;
        ofn.initialDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        ofn.title = "Open image";
        ofn.defExt = "PNG";
        ofn.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;

        if (DllTest.GetOpenFileName(ofn))
            return ofn.file;

        return "";
    }

    public static string SaveFileDialog(string nameBook)
    {
        SaveFileDlg sfd = new SaveFileDlg();
        sfd.structSize = Marshal.SizeOf(sfd);
        sfd.filter = "PDF(*.pdf)\0*.pdf";
        sfd.file = new string(new char[256]);
        sfd.maxFile = sfd.file.Length;
        sfd.fileTitle = new string(new char[64]);
        sfd.maxFileTitle = sfd.fileTitle.Length;
        sfd.initialDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        sfd.title = "Save Book";
        sfd.defExt = "PDF";
        sfd.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;

        if (DllTest.GetSaveFileName(sfd))
            return sfd.file;

        return "";
    }
}


