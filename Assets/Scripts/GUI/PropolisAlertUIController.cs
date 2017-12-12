using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class PropolisAlertUIController : MonoBehaviour {
    public float FadeInTime = 0.3f;
    public float ShowingTime = 4.0f;
    public float FadeOutTime = 0.3f;
    [SerializeField]
    public Text TitleTextUI;
    public Text ContentTextUI;
    public CanvasGroup CanvasGroup;
    public RectTransform rectTransform;
    // Use this for initialization
    void Start () {
        CanvasGroup.alpha = 0.0f;
    }



    public void Show(string TitleText, string ContentText)
    {
        TitleTextUI.text = TitleText;
        ContentTextUI.text = ContentText+"...";
        CanvasGroup.alpha = 0;
        rectTransform.localPosition = new Vector3(rectTransform.localPosition.x, -Screen.currentResolution.height*.5f, 0.0f);
        StopCoroutine(StartShowing());
        StartCoroutine(StartShowing());
    }


    


    private IEnumerator StartShowing()
    {
        StartCoroutine(FadeIn());
        yield return new WaitForSeconds(ShowingTime);
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeIn()
    {

        while (CanvasGroup.alpha < 1.0f)
        {
            CanvasGroup.alpha += 0.01f;
            rectTransform.localPosition += new Vector3(0.0f, 1f,0.0f);
            yield return new WaitForSeconds(0.005f);

        }
    }

    private IEnumerator FadeOut()
    {
       while (CanvasGroup.alpha > 0.0f)
        {
            CanvasGroup.alpha -=  0.005f;
            rectTransform.localPosition -= new Vector3(0.0f, -.05f, 0.0f);
            yield return new WaitForSeconds(0.001f);

        }

        rectTransform.localPosition = new Vector3(rectTransform.localPosition.x, -Screen.currentResolution.height * .5f, 0.0f);

    }

    // Update is called once per frame
    void Update () {
		
	}
}
