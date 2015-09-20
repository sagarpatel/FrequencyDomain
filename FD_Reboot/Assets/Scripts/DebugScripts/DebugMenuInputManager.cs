using UnityEngine;
using System.Collections;
using InControl;

public class DebugMenuControls : PlayerActionSet
{
	public PlayerAction Left;
	public PlayerAction Right;
	public PlayerOneAxisAction Horizontal;
	public PlayerAction Down;
	public PlayerAction Up;
	public PlayerOneAxisAction Vertical;
	public PlayerAction Enter;
	public PlayerAction Back;

	public DebugMenuControls()
	{
		Left = CreatePlayerAction( "Menu Left" );
		Right = CreatePlayerAction( "Menu Right" );
		Horizontal = CreateOneAxisPlayerAction(Left, Right);
		Up = CreatePlayerAction( "Menu Up" );
		Down = CreatePlayerAction( "Menu Down" );
		Vertical = CreateOneAxisPlayerAction(Down, Up);
		Enter = CreatePlayerAction("Enter");
		Back = CreatePlayerAction("Back");
	}


}

public class DebugMenuInputManager : MonoBehaviour 
{
	DebugMenuNavigator m_debugMenuNavigator;
	DebugMenuControls m_debugMenuControls;

	void Start()
	{
		m_debugMenuNavigator = FindObjectOfType<DebugMenuNavigator>();

		// keyboard controls
		m_debugMenuControls = new DebugMenuControls();
		m_debugMenuControls.Left.AddDefaultBinding( Key.LeftArrow );
		m_debugMenuControls.Right.AddDefaultBinding( Key.RightArrow );
		m_debugMenuControls.Up.AddDefaultBinding( Key.UpArrow );
		m_debugMenuControls.Down.AddDefaultBinding( Key.DownArrow );
	}

	void Update()
	{
		// expected values : -1 or 1, 0 will be discarded
		int horizontalInput = (int)m_debugMenuControls.Horizontal.Value;
		int verticalInput = (int)m_debugMenuControls.Vertical.Value;

		if(horizontalInput != 0 )
			m_debugMenuNavigator.HandleInput_LeftRight(horizontalInput);

		if(verticalInput != 0)
			m_debugMenuNavigator.HandleInput_UpDown(verticalInput);

	}

}
