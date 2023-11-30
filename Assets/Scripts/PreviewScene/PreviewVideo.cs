using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class PreviewVideo : MonoBehaviour
{
    [SerializeField] private int _numberLoadScene;
    private VideoPlayer videoPlayer;

    private void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();

        if (!videoPlayer.isLooping)
            videoPlayer.isLooping = true;

        videoPlayer.loopPointReached += OnVideoEnd;
    }

    void OnDisable() 
    {
        videoPlayer.loopPointReached -= OnVideoEnd;
    }

    void OnVideoEnd(VideoPlayer causedVideoPlayer)
    {
        videoPlayer.isLooping = false;

        if (PlayerPrefs.HasKey("Tutorial_Completed"))
            _numberLoadScene += 1;

        SceneManager.LoadScene(_numberLoadScene);
    }
}
