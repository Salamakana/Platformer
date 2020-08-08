using UnityEngine;

public class AudioSyncer : MonoBehaviour
{
    [SerializeField]
    private float bias;
    [SerializeField]
    private float timeStep;
    [SerializeField]
    protected float timeToBeat;
    [SerializeField]
    protected float restSmoothTime;

    private float previousAudioValue;
    private float audioValue;
    private float timer;

    protected bool isBeat = false;

    private void Update()
    {
        OnUpdate();
    }

    protected virtual void OnBeat()
    {
        timer = 0f;
        isBeat = true;
    }

    protected virtual void OnUpdate()
    {
        previousAudioValue = audioValue;
        audioValue = AudioSpectrum.Instance.SpectrumValue;

        if (previousAudioValue > bias && audioValue <= bias)
        {
            if (timer > timeStep)
            {
                OnBeat();
            }
        }

        if (previousAudioValue <= bias && audioValue > bias)
        {
            if (timer > timeStep)
            {
                OnBeat();
            }
        }

        timer += Time.deltaTime;
    }
}
