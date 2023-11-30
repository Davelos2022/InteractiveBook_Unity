using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public static class SaveInfo
{
    public struct PageInfo
    {
        public static List<Sprite> _sprite = new List<Sprite>();
        public static List<TMP_InputField> _textMeshes = new List<TMP_InputField>();
        public static int _indexPage;
    }

    public static List<PageInfo> pageInfos = new List<PageInfo>(30);

}