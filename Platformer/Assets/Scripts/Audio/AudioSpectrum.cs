using UnityEngine;

public class AudioSpectrum : Singelton<AudioSpectrum>
{
	private float[] audioSpectrum = new float[128];
	public float SpectrumValue { get; private set; }

	private void Update()
	{
		AudioListener.GetSpectrumData(audioSpectrum, 0, FFTWindow.Hamming);

		if (audioSpectrum != null && audioSpectrum.Length > 0)
		{
			SpectrumValue = audioSpectrum[0] * 100;
		}
	}
}
