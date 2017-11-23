using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Propolis;
using UnityEngine.UI;


public class BatteryUiController : MonoBehaviour {

    public Slider battery;
    public Image fill, backgroud;

	void Start () {
        fill.color = PropolisColors.Purple;
        backgroud.color = PropolisColors.Dark;
    }

    public void BatteryValueUpdate(float BatteryLevel)
    {
        StartCoroutine(SlideToValue(BatteryLevel,1f));
    }

    public IEnumerator SlideToValue(float ToValue,float time)
    {
        float elapsedTime = 0;
        var currentValue = battery.value;

        while (elapsedTime < time)
        {
            battery.value = Mathf.Lerp(currentValue, ToValue, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}