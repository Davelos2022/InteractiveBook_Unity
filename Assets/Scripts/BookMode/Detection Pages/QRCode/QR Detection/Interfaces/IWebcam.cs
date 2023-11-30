using UnityEngine;

namespace BarcodeScanner
{
    public enum TypeCamera { Left, Right }

    public interface IWebcam
    {
        // 
        Texture Texture { get; }
        WebCamTexture WebCamTexture { get; set; }
        Texture2D LeftOutput { get; set; }
        Texture2D RightOutput { get; set; }
        int CropWidth { get; set; }
        int CropHeight { get; set; }
        int Width { get; }
        int Height { get; }

        //
        void SetSize();
        bool IsReady();
        bool IsPlaying();
        void Play();
        void Stop();
        void Destroy();

        //
        Color32[] GetPixels(TypeCamera typeCamera, Color32[] data);
        Color32[] ConvetToColor32(Texture2D texture);

        float GetRotation();
        bool IsVerticalyMirrored();
        Vector3 GetEulerAngles();
        Vector3 GetScale();
        int GetChecksum();
    }
}