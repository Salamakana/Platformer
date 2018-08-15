using UnityEngine;

public class MusicPlayer : Singelton<MusicPlayer>
{
	public MusicTrack[] MusicTracks;

	private void Start ()
	{
		CreateMusicSourceContainers();
		PlayRandomMusicTrack();
	}

	private void CreateMusicSourceContainers()
	{
		GameObject musicTracksConteiner = AudioManager.Instance.CreateNewGameObjectContainer("Music", transform);

		foreach (var musicTrack in MusicTracks)
		{
			GameObject musicSourceObject = new GameObject();
			musicTrack.SetSource(musicSourceObject.AddComponent<AudioSource>(), AudioManager.Instance.GetAudioMixerOutputChannel("Music"));
			musicSourceObject.name = musicTrack.SoundName;
			musicSourceObject.transform.SetParent(musicTracksConteiner.transform);
		}
	}

	public void ChangeRandomMusicTrack()
	{
		StopAllMusicTracks();
		PlayRandomMusicTrack();
	}

	public void PlayMusicTrack(string trackName)
	{
		foreach (var musicTrack in MusicTracks)
		{
			if (musicTrack.SoundName.Equals(trackName))
			{
				musicTrack.Play();
			}
		}
	}

	public void PlayRandomMusicTrack()
	{
		var randomValue = Random.Range(0, MusicTracks.Length);

		MusicTracks[randomValue].Play();
	}

	public void StopMusicTrack(string trackName)
	{
		foreach (var musicTrack in MusicTracks)
		{
			if (musicTrack.SoundName.Equals(trackName))
			{
				musicTrack.Stop();
			}
		}
	}

	public void StopAllMusicTracks()
	{
		foreach (var musicTrack in MusicTracks)
		{
			musicTrack.Stop();
		}
	}
}
