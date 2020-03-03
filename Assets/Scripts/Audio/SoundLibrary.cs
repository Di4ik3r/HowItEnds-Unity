using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundLibrary : MonoBehaviour
{
	public SoundGroup[] soundGroups;
	public Music[] music;

	Dictionary<string, AudioClip[]> groupDictionary = new Dictionary<string, AudioClip[]>();
	Dictionary<string, AudioClip> musicDictionary = new Dictionary<string, AudioClip>();

	void Awake()
	{
		foreach (SoundGroup soundGroup in soundGroups)
		{
			groupDictionary.Add(soundGroup.groupID, soundGroup.group);
		}

		foreach (Music track in music)
		{
			musicDictionary.Add(track.Name, track.AudioClip);
		}
	}

	public AudioClip GetClipFromName(string name, int number = 0)
	{
		if (groupDictionary.ContainsKey(name))
		{
			AudioClip[] sounds = groupDictionary[name];
			return sounds[number];
		}
		return null;
	}

	public AudioClip GetMusicFromName(string name)
	{
		if (musicDictionary.ContainsKey(name))
		{
			return musicDictionary[name];
		}
		return null;
	}

	[System.Serializable]
	public class SoundGroup
	{
		public string groupID;
		public AudioClip[] group;
	}

	[System.Serializable]
	public class Music
	{
		public string Name;
		public AudioClip AudioClip;
	}
}
