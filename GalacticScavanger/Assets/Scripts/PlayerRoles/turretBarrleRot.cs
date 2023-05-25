using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class turretBarrleRot : MonoBehaviour
{
    public float startValue = 3f;
    public float targetValue = 8f;
    public float duration = 5f;
    public AnimationCurve easingCurve;
    float currentValue;

    private float elapsedTime = 0f;

    private void Start()
    {
        elapsedTime = 0;
    }
    private void OnEnable()
    {
        elapsedTime = 0;
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;

            float t = elapsedTime / duration; // Normalized time between 0 and 1
            float easedT = easingCurve.Evaluate(t); // Apply easing curve to the time value

            currentValue = Mathf.Lerp(startValue, targetValue, easedT);


        transform.Rotate(currentValue, 0, 0, Space.Self);
    }

}
