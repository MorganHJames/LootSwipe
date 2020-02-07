//\===========================================================================================
//\ File: SwipeManager.cs
//\ Author: Morgan James
//\ Brief: Allows for the detection of swiping as input.
//\===========================================================================================

using UnityEngine;

public enum SwipeDirection
{
	None = 0,
	Left = 1,
	Right = 2,
	Up = 4,
	Down = 8,
}

public class SwipeManager : MonoBehaviour
{
	//Simple Singleton pattern.
	public static SwipeManager instance;

	private void Awake()
	{
		//Make sure there is only one SwipeManager.
		if (instance == null)
			instance = this;
	}

	private Vector3 m_v3TouchPosition;//This initial touch position.
	private float m_fSwipeResistanceX = 50.0f;//How much the player has to swipe left or right for it to count.
	private float m_fSwipeResistanceY = 75.0f;//How much the player has to swipe up or down for it to count.

	[HideInInspector]
	public SwipeDirection m_eDirection;//The direction the player is swiping.

	private void Update()
	{
		m_eDirection = SwipeDirection.None;//Set the current direction swiping to be equal to nothing.

		if (Input.GetMouseButtonDown(0))//When the mouse is pressed of the screen is touched.
		{
			m_v3TouchPosition = Input.mousePosition;//Set the initial position of the touch.
		}

		if (Input.GetMouseButtonUp(0))//If the mouse is release or the player stopped touching the screen.
		{
			Vector2 v2DeltaSwipe = m_v3TouchPosition - Input.mousePosition;//Get the vector of the swipe.

			if (Mathf.Abs(v2DeltaSwipe.x) > m_fSwipeResistanceX)
			{
				//Swipe on the X axis.
				m_eDirection |= (v2DeltaSwipe.x < 0) ? SwipeDirection.Right : SwipeDirection.Left;

			}

			if (Mathf.Abs(v2DeltaSwipe.y) > m_fSwipeResistanceY)
			{
				//Swipe on the Y axis.
				m_eDirection |= (v2DeltaSwipe.y < 0) ? SwipeDirection.Up : SwipeDirection.Down;
			}
		}
	}

	//A function to indicate if a player is swiping in a certain direction.
	public bool IsSwiping(SwipeDirection a_eSwipeDirection)
	{
		if (m_eDirection == a_eSwipeDirection)
		{
			return true;
		}
		else
		{
			return false;
		}
	}
}

