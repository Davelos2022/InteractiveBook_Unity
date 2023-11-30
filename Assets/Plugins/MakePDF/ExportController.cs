using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using UnityEngine;
using UnityEngine.UI;


    public class ExportController 
    {
        private readonly string _tempDeployPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\tempFolder";
        private readonly string _deploymentPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\ALMA\\SocialStories\\Export";


        void MakeDocument()
        {
            var doc = new Document(PageSize.A3);
            if (!Directory.Exists(_deploymentPath)) Directory.CreateDirectory(_deploymentPath);
            var dateExtension = DateTime.Now;
            PdfWriter.GetInstance(doc, new FileStream($"{_deploymentPath}\\33_{dateExtension.ToFileTimeUtc()}.pdf", FileMode.Create));
            doc.Open();

           
        }
    }
