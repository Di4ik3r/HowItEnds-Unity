using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	public enum AudioChannel { Master, Sfx, Music };

	private Dictionary<string, int> Music = new Dictionary<string, int>();

	float masterVolumePercent = 1f;
	float sfxVolumePercent = 1;
	float musicVolumePercent = 0.1f;

	AudioSource sfx2DSource;
	List<AudioSource> musicSources;
	int activeMusicSourceIndex = -1;

	public static AudioManager instance;

	Transform audioListener;

	SoundLibrary library;
	bool pause = false;

	void Awake()
	{
		if (instance != null)
		{
			Destroy(gameObject);
		}
		else
		{

			instance = this;
			DontDestroyOnLoad(gameObject);

			library = GetComponent<SoundLibrary>();

			musicSources = new List<AudioSource>();

			GameObject newSfx2Dsource = new GameObject("2D sfx source");
			sfx2DSource = newSfx2Dsource.AddComponent<AudioSource>();
			newSfx2Dsource.transform.parent = transform;

			audioListener = FindObjectOfType<AudioListener>().transform;			

			masterVolumePercent = PlayerPrefs.GetFloat("MasterVol", masterVolumePercent);
			musicVolumePercent = PlayerPrefs.GetFloat("MusicVol", musicVolumePercent);
			sfxVolumePercent = PlayerPrefs.GetFloat("SfxVol", sfxVolumePercent);
		}
	}

	public void SetVolume(float volumePercent, AudioChannel channel)
	{
		switch (channel)
		{
			case AudioChannel.Master:
				masterVolumePercent = volumePercent;
				break;
			case AudioChannel.Sfx:
				sfxVolumePercent = volumePercent;
				break;
			case AudioChannel.Music:
				musicVolumePercent = volumePercent;
				break;
		}

		musicSources.ForEach(src => src.volume = musicVolumePercent * masterVolumePercent);		

		PlayerPrefs.SetFloat("MasterVol", masterVolumePercent);
		PlayerPrefs.SetFloat("MusicVol", musicVolumePercent);
		PlayerPrefs.SetFloat("SfxVol", sfxVolumePercent);
		PlayerPrefs.Save();
	}

	public void PlayMusic(string musicName, float fadeDuration = 1)
	{
		string objectName = "Music source " + (++activeMusicSourceIndex);
		
		if (Music.ContainsKey(musicName))
		{
			UnPauseMusic(musicName, 2);
		}
		else
		{
			GameObject newMusicSource = new GameObject(objectName);
			musicSources.Add(newMusicSource.AddComponent<AudioSource>());
			newMusicSource.transform.parent = transform;
			
			Music.Add(musicName, activeMusicSourceIndex);

			musicSources[activeMusicSourceIndex].clip = library.GetMusicFromName(musicName);
			musicSources[activeMusicSourceIndex].loop = true;
			musicSources[activeMusicSourceIndex].Play();
		}

		StartCoroutine(AnimateMusicCrossfade(fadeDuration));

	}

	public void PauseMusic(string musicName, float fadeDuration = 1)
	{
		pause = true;

		int id = Music[musicName];
		
		musicSources[id].Pause();

		StartCoroutine(AnimateActiveMusic(fadeDuration));
	}

	public void UnPauseMusic(string musicName, float fadeDuration = 1)
	{
		int id = Music[musicName];
		activeMusicSourceIndex = id;		
		musicSources[id].UnPause();

		StartCoroutine(AnimateActiveMusic(fadeDuration));
	}

	public void PlaySound(AudioClip clip, Vector3 pos)
	{
		if (clip != null)
		{
			AudioSource.PlayClipAtPoint(clip, pos, sfxVolumePercent * masterVolumePercent);
		}
	}

	public void PlaySound(string soundName, Vector3 pos)
	{
		PlaySound(library.GetClipFromName(soundName), pos);
	}

	public void PlaySound2D(string soundName)
	{
		sfx2DSource.PlayOneShot(library.GetClipFromName(soundName), sfxVolumePercent * masterVolumePercent);
	}

	IEnumerator AnimateActiveMusic(float duration)
	{
		float percent = 0;
		float time = 0.01f + Time.deltaTime;
		if (pause)
		{
			musicSources[activeMusicSourceIndex].volume = 0f;
			pause = false;
		}
		else
		{
			while (percent < 1)
			{
				percent += time * 1 / duration;
				musicSources[activeMusicSourceIndex].volume = Mathf.Lerp(0, musicVolumePercent * masterVolumePercent, percent);
			
				yield return null;
			}
		}
	}

	IEnumerator AnimateMusicCrossfade(float duration)
	{
		float percent = 0;					
		float time = 0.01f + Time.deltaTime;

		while (percent < 1)
		{				
			percent += time * 1 / duration;
			musicSources[activeMusicSourceIndex].volume = Mathf.Lerp(0, musicVolumePercent * masterVolumePercent, percent);			
			yield return null;
		}				
	}
}
