using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
	[SerializeField] GameObject audioSourcePrefab;
	[SerializeField] Queue<AudioSource> _audioPool = new Queue<AudioSource>();

	AudioSource GetAudioSource()
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

		source.Stop();
		source.gameObject.SetActive(false);
		_audioPool.Enqueue(source); 
	}

	public void PlayAudioClip(AudioClip audio)
	{
		AudioSource ads = GetAudioSource();
		ads.clip = audio;
		ads.Play(); 
		StartCoroutine(ReturnAudioSource(ads, audio.length));
	}
}
