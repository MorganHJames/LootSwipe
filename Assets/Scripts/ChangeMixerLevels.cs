//\===========================================================================================
//\ File: ChangeMixerLevels.cs
//\ Author: Morgan James
//\ Brief: Allows for the changing of exposed audio mixer variables.
//\===========================================================================================

using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]//Make sure the game object has a slider also attached.
public class ChangeMixerLevels : MonoBehaviour
{
	[SerializeField]
	private AudioMixer m_Mixer;//The mixer that contains the variables we want to change.

	[SerializeField]
	private string m_sParameterName;//The parameter we want to change.

	//Changes the sound level of a parameter based on its input.
	public void SetMusicLevel(float a_musicLevel)
	{
		m_Mixer.SetFloat(m_sParameterName, a_musicLevel);//Sets the level of the chosen parameter.
	}

	//Sets the slider value equal to the current value as there are multiple volume sliders that change the same variable.
	private void OnEnable()
	{
		float fTemp = 0;//Create a temporary float.
		m_Mixer.GetFloat(m_sParameterName, out fTemp);//Set the float equal to the current value of the parameter.
		GetComponent<Slider>().value = fTemp;//Set the slider value equal to the parameter.
	}
}