using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AnimShowPage : MonoBehaviour
{
    [SerializeField] private float _timeAnim;
    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
        image.material = Instantiate(image.material);
    }

    private IEnumerator AnimMaterial()
    {
        float alpha = 0;
        float stack = 0;

        while (alpha < 1)
        {
            stack += Time.deltaTime / _timeAnim;
            alpha = Mathf.Lerp(0, 1, stack);
            image.material.SetFloat("_Open", alpha);
            yield return null;
        }

    }
    private void OnEnable()
    {
        StopCoroutine(AnimMaterial());

        StartCoroutine(AnimMaterial());
    }
}
