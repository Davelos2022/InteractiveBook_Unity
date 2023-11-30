using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace VolumeBox.Toolbox.UIInformer
{
    public class Info : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI okBTN;
        [SerializeField] private TextMeshProUGUI cancelBTN;
        [SerializeField] private TextMeshProUGUI selestText;
        [SerializeField] private TextMeshProUGUI TitleBTN;
        [SerializeField] private MessageBox messageBox;
        [SerializeField] private HintBox hintBox;


        public static Info Instance;
        private BoxBase _currentOpenedBox;


        private void Awake()
        {
            Instance = this;
        }
        public void ShowBox(string message, Action okAction = null, Action cancelAction = null, string okText = null, string cancelText = null, string title = null)
        {
            if (okText != null)
            {
                okBTN.text = okText;
                selestText.text = okText;
            }
            if (cancelText != null)
                cancelBTN.text = cancelText;
            if (title != null)
                TitleBTN.text = title;

            messageBox.SetMessage(message);
            messageBox.SetCancelAction(cancelAction);
            messageBox.SetOkAction(okAction);
            
            if (messageBox.Show())
            {
                _currentOpenedBox = messageBox;
            }
        }

        public void ShowHint(string message, float? delay = null)
        {
            if (delay.HasValue)
            {
                hintBox.SetDelay(delay.Value);
            }
            
            hintBox.SetMessage(message);
            hintBox.Show();
        }

        public void OffMessage()
        {
            messageBox.CancelClick();
            messageBox.Close();
        }

    }
}
