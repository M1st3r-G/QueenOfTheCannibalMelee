using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthbar : MonoBehaviour
{
    //ComponentReferences
    [SerializeField] private Gradient gradient;
    [SerializeField] private float waitTime;
    [SerializeField] private Image fill;
    [SerializeField] private Slider s;
    [SerializeField] private CanvasGroup c;
    //Params
    //Temps
    private Coroutine fadeOut;
    //Publics
    
    public void SetValue(int amount)
    {
        s.value = amount;
        fill.color = gradient.Evaluate(amount / s.maxValue);
        if(fadeOut is not null) StopCoroutine(fadeOut);
        c.alpha = 1f;
        StartCoroutine(FadeOut());
    }

    public void SetMaxAndMin(int max, int min)
    {
        s.minValue = min;
        s.maxValue = max;
        s.value = max;
    }

    private IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(waitTime);

        while (c.alpha > 0)
        {
            c.alpha -= Time.deltaTime;
            yield return null;
        }
    }
}