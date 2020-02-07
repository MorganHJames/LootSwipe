//\===========================================================================================
//\ File: PlayOnShotAudio.cs
//\ Author: Morgan James
//\ Brief: Contains the functionality to play a sound (Button click).
//\===========================================================================================

using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]//Make sure the game object has an AudioSource also attached.
public class PlayOneShotAudio : MonoBehaviour
{
	[SerializeField]
	private AudioClip m_AudioClip;//The audio clip that will play.
	private AudioSource m_AudioSource;//The audio source of which the sound will play from.

	void Start()
	{
		m_AudioSource = GetComponent<AudioSource>();//Set the audio source.
	}

	public void PlayOneShot()
	{
		m_AudioSource.PlayOneShot(m_AudioClip, 1.0F);//Plays the sound.
	}
}
