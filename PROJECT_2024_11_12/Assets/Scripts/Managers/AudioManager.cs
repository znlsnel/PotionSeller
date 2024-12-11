using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
	[SerializeField] GameObject audioSourcePrefab;
	Queue<AudioSource> _audioPool = new Queue<AudioSource>();

	public AudioSource GetAudioSource()
	{
		if (_audioPool.Count > 0)
		{
			var source = _audioPool.Dequeue();
			source.gameObject.SetActive(true);
			return source;
		}
		else
		{
			var newSource = Instantiate(audioSourcePrefab, transform).GetComponent<AudioSource>();
			return newSource;
		}
	}

	IEnumerator ReturnAudioSource(AudioSource source, float delay)
	{
		yield return new WaitForSeconds(delay);

		ReturnAudioSource(source); 
	}

	public void ReturnAudioSource(AudioSource source)
	{
		source.Stop();
		source.gameObject.SetActive(false);
		_audioPool.Enqueue(source);
	}

	public void PlayAudioClip(AudioClip audio, float size = 1.0f)
	{
		if (audio == null)
			return;

		AudioSource ads = GetAudioSource();
		ads.volume = size;
		ads.clip = audio;
		ads.Play(); 
		StartCoroutine(ReturnAudioSource(ads, audio.length));
	}

	public void PlayAudioClip(List<AudioClip> audios, float size = 1.0f)
	{
		if (audios == null || audios.Count == 0)
			return;
		 
		AudioClip audio = audios[Random.Range(0, audios.Count)]; 
		AudioSource ads = GetAudioSource();
		ads.volume = size;
		ads.clip = audio;
		ads.Play();
		StartCoroutine(ReturnAudioSource(ads, audio.length));
	}
}
