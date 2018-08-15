using UnityEngine;

public class SfxLibrary : Singelton<SfxLibrary>
{
	public Sfx[] UIEffects;
	public Sfx[] GameEffects;

	private void Start()
	{
		CreateSfxSourceContainers(UIEffects, "UIEffects");
		CreateSfxSourceContainers(GameEffects, "GaneEffects");
	}

	private void CreateSfxSourceContainers(Sfx[] effectTypes, string containerName)
	{
		GameObject sfxConteiner = AudioManager.Instance.CreateNewGameObjectContainer(containerName, transform);


		foreach (var UIEffect in effectTypes)
		{
			GameObject musicSourceObject = new GameObject();
			UIEffect.SetSource(musicSourceObject.AddComponent<AudioSource>(), AudioManager.Instance.GetAudioMixerOutputChannel("Sfx"));
			musicSourceObject.name = UIEffect.SoundName;
			musicSourceObject.transform.SetParent(sfxConteiner.transform);
		}
	}

	private AudioClip GetClipFromArray(AudioClip[] audioClips, string audioClipName)
	{
		foreach (var audioClip in audioClips)
		{
			if (audioClip.name.Equals(audioClip))
			{
				return audioClip;
			}
		}

		Debug.LogWarning("No clip found by name: " + audioClipName);
		return null;
	}

	public void PlayUISfx(string sfxName)
	{
		foreach (var sfx in UIEffects)
		{
			if (sfx.SoundName.Equals(sfxName))
			{
				sfx.Play();
				return;
			}
		}

		Debug.LogWarning("No sound found by name of: " + sfxName);
	}

	public void PlayGameSfx(string sfxName)
	{
		foreach (var sfx in GameEffects)
		{
			if (sfx.SoundName.Equals(sfxName))
			{
				sfx.Play();
				return;
			}			
		}

		Debug.LogWarning("No sound found by name of: " + sfxName);
	}
}
