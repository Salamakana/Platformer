using System.Collections;
using UnityEngine;

public class AudioSyncScale : AudioSyncer
{
    private Vector3 currentScale;
    private Vector3 initialScale;
    private Vector3 beatScale = new Vector3(1.5f, 1.5f, 1.5f);
    private Vector3 restScale = Vector3.one;

    private IEnumerator IMoveToScale(Vector3 targetScale)
    {
        currentScale = transform.localScale;
        initialScale = currentScale;
        var timer = 0f;

        while(currentScale != targetScale)
        {
            currentScale = Vector3.Lerp(initialScale, targetScale, timer / timeToBeat);
            timer += Time.deltaTime;
            transform.localScale = currentScale;
            yield return null;
        }

        isBeat = false;
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        if (isBeat)
            return;

        transform.localScale = Vector3.Lerp(transform.localScale, restScale, restSmoothTime * Time.deltaTime);
    }

    protected override void OnBeat()
    {
        base.OnBeat();

        StopCoroutine("IMoveToScale");
        StartCoroutine("IMoveToScale", beatScale);
    }
}
