using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VolumeBox.Toolbox.UIInformer;


public class WebCamSettings : MonoBehaviour
{
    [Header("Screen Settings")]
    public int Width;
    public int Height;
    public int FrameRate;
    [Header("Crop Settings")]
    public int _widthCrop;
    public int _heightCrop;
    [Header("Sliders Left Output")]
    public Slider CropXLeftSlider;
    public Slider CropYLeftSlider;
    [Header("Sliders Right Output")]
    public Slider CropXRightSlider;
    public Slider CropYRightSlider;
    [Space]
    [Header("TreshHold sliders")]
    public Slider _thresholdSlider;
    public Slider _maskSlider;
    public Slider _outMasSlider;
    [Space]
    [Header("Text Links left sliders")]
    [SerializeField] private TextMeshProUGUI LefTextX;
    [SerializeField] private TextMeshProUGUI LeftTextY;
    [Header("Text Links Right sliders")]
    [SerializeField] private TextMeshProUGUI RightTextX;
    [SerializeField] private TextMeshProUGUI RightTextY;
    [Header("Text Links Threshhold sliders")]
    [SerializeField] private TextMeshProUGUI _textTreshold;
    [SerializeField] private TextMeshProUGUI _maskSliderText;
    [SerializeField] private TextMeshProUGUI _outMasSliderText;

    [Header("Debug and settings detection text")]
    [SerializeField] private GameObject[] cropPositionObjects;
    [SerializeField] private GameObject[] ThresholdObjects;

    private bool _isSettings;
    public TypeDetection TypeDetection { get; set; }

    void Start()
    {
        LoadSettings();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.LeftControl) && CheckUser.Instance.Authorization)
            SettingsPanel(_isSettings = !_isSettings);

        if (_isSettings)
            UpdateValueSlider();
    }

    private void SaveSettings()
    {

    }

    private void LoadSettings()
    {
        SettingsPanel(false);
    }

    private void SettingsPanel(bool active)
    {
        _isSettings = active;

        for (int x = 0; x < cropPositionObjects.Length; x++)
            cropPositionObjects[x].SetActive(_isSettings);

        if (TypeDetection == TypeDetection.TesseractMode)
        {
            for (int x = 0; x < cropPositionObjects.Length; x++)
                ThresholdObjects[x].SetActive(_isSettings);
        }
    }

    private void UpdateValueSlider()
    {
        LefTextX.text = $"{(int)CropXLeftSlider.value}";
        LeftTextY.text = $"{(int)CropYLeftSlider.value}";

        RightTextX.text = $"{(int)CropXRightSlider.value}";
        RightTextY.text = $"{(int)CropYRightSlider.value}";

        _textTreshold.text = $"{(int)_thresholdSlider.value}";
        _maskSliderText.text = $"{(int)_maskSlider.value}";
        _outMasSliderText.text = $"{(int)_outMasSlider.value}";
    }

    public void NotCamError()
    {
        Info.Instance.ShowBox("Камера не найдена или не подключена",
                             null, ReturnInLibarry, null, "Понятно");
    }

    private void ReturnInLibarry()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene
            (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}
