using UnityEngine;

[System.Serializable]
public class Sfx : Sound
{
    [Range(0, 1)]
    public float RandomVolume = 0f;
    [Range(0, 0.5f)]
    public float RandomPitch = 0f;

    public override void Play()
    {
        audioSource.volume = Volume * (1 + Random.Range(-RandomVolume / 2f, RandomVolume / 2f));
        audioSource.pitch = Pitch * (1 + Random.Range(-RandomPitch / 2f, RandomPitch / 2f)); ;
        base.Play();
    }
}