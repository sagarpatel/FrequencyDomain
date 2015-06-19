using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using InControl;


namespace BindingsExample
{
	public class PlayerActions : PlayerActionSet
	{
		public PlayerAction Fire;
		public PlayerAction Jump;
		public PlayerAction Left;
		public PlayerAction Right;
		public PlayerAction Up;
		public PlayerAction Down;
		public PlayerTwoAxisAction Move;


		public PlayerActions()
		{
			Fire = CreatePlayerAction( "Fire" );
			Jump = CreatePlayerAction( "Jump" );
			Left = CreatePlayerAction( "Move Left" );
			Right = CreatePlayerAction( "Move Right" );
			Up = CreatePlayerAction( "Move Up" );
			Down = CreatePlayerAction( "Move Down" );
			Move = CreateTwoAxisPlayerAction( Left, Right, Down, Up );
		}
	}


	public class CubeController : MonoBehaviour
	{
		Renderer cachedRenderer;
		PlayerActions playerInput;
		string saveData;


		void OnEnable()
		{
			playerInput = new PlayerActions();

			playerInput.Fire.AddDefaultBinding( Key.Shift, Key.A );
			playerInput.Fire.AddDefaultBinding( InputControlType.Action1 );
			playerInput.Fire.AddDefaultBinding( Mouse.LeftButton );
			playerInput.Fire.AddDefaultBinding( Mouse.PositiveScrollWheel );

			playerInput.Jump.AddDefaultBinding( Key.Space );
			playerInput.Jump.AddDefaultBinding( InputControlType.Action3 );
			playerInput.Jump.AddDefaultBinding( InputControlType.Back );
			playerInput.Jump.AddDefaultBinding( InputControlType.System );
			playerInput.Jump.AddDefaultBinding( Mouse.NegativeScrollWheel );

			playerInput.Up.AddDefaultBinding( Key.UpArrow );
			playerInput.Down.AddDefaultBinding( Key.DownArrow );
			playerInput.Left.AddDefaultBinding( Key.LeftArrow );
			playerInput.Right.AddDefaultBinding( Key.RightArrow );

			playerInput.Left.AddDefaultBinding( InputControlType.LeftStickLeft );
			playerInput.Right.AddDefaultBinding( InputControlType.LeftStickRight );
			playerInput.Up.AddDefaultBinding( InputControlType.LeftStickUp );
			playerInput.Down.AddDefaultBinding( InputControlType.LeftStickDown );

			playerInput.Up.AddDefaultBinding( Mouse.PositiveY );
			playerInput.Down.AddDefaultBinding( Mouse.NegativeY );
			playerInput.Left.AddDefaultBinding( Mouse.NegativeX );
			playerInput.Right.AddDefaultBinding( Mouse.PositiveX );

			playerInput.ListenOptions.MaxAllowedBindings = 3;
//			playerInput.ListenOptions.MaxAllowedBindingsPerType = 1;

			playerInput.ListenOptions.OnBindingFound = ( action, binding ) =>
			{
				if (binding == new KeyBindingSource( Key.Escape ))
				{
					action.StopListeningForBinding();
					return false;
				}
				return true;
			};

			playerInput.ListenOptions.OnBindingAdded += ( action, binding ) =>
			{
				Debug.Log( "Binding added... " + binding.DeviceName + ": " + binding.Name );
			};

			playerInput.ListenOptions.OnBindingRejected += ( action, binding, reason ) =>
			{
				Debug.Log( "Binding rejected... " + reason );
			};

			LoadBindings();
		}


		void Start()
		{
			cachedRenderer = GetComponent<Renderer>();
//			playerInput.Enabled = false;
		}


		void Update()
		{
			transform.Rotate( Vector3.down, 500.0f * Time.deltaTime * playerInput.Move.X, Space.World );
			transform.Rotate( Vector3.right, 500.0f * Time.deltaTime * playerInput.Move.Y, Space.World );

			var fireColor = playerInput.Fire.IsPressed ? Color.red : Color.white;
			var jumpColor = playerInput.Jump.IsPressed ? Color.green : Color.white;

			cachedRenderer.material.color = Color.Lerp( fireColor, jumpColor, 0.5f );

			if (playerInput.Fire.WasPressed)
				Debug.Log( "Pressed" );

			if (playerInput.Fire.WasReleased)
				Debug.Log( "Released" );
		}


		void SaveBindings()
		{
			saveData = playerInput.Save();
			PlayerPrefs.SetString( "Bindings", saveData );
		}


		void LoadBindings()
		{
			if (PlayerPrefs.HasKey( "Bindings" ))
			{
				saveData = PlayerPrefs.GetString( "Bindings" );
				playerInput.Load( saveData );
			}
		}


		void OnApplicationQuit()
		{
			PlayerPrefs.Save();
		}


		void OnDrawGizmos()
		{
			Gizmos.color = Color.blue;
			var lz = new Vector2( -3.0f, -1.0f );
			var lp = lz + (InputManager.ActiveDevice.Direction.Vector * 2.0f);
			Gizmos.DrawSphere( lz, 0.1f );
			Gizmos.DrawLine( lz, lp );
			Gizmos.DrawSphere( lp, 1.0f );

			Gizmos.color = Color.red;
			var rz = new Vector2( +3.0f, -1.0f );
			var rp = rz + (InputManager.ActiveDevice.RightStick.Vector * 2.0f);
			Gizmos.DrawSphere( rz, 0.1f );
			Gizmos.DrawLine( rz, rp );
			Gizmos.DrawSphere( rp, 1.0f );
		}


		void OnGUI()
		{
			const float h = 22.0f;
			var y = 10.0f;

			GUI.Label( new Rect( 10, y, 300, y + h ), "Last Input Type: " + playerInput.LastInputType.ToString() );
			y += h;

			var actionCount = playerInput.Actions.Count;
			for (int i = 0; i < actionCount; i++)
			{
				var action = playerInput.Actions[i];

				var name = action.Name;
				if (action.IsListeningForBinding)
				{
					name += " (Listening)";
				}
				GUI.Label( new Rect( 10, y, 300, y + h ), name );
				y += h;

				var bindingCount = action.Bindings.Count;
				for (int j = 0; j < bindingCount; j++)
				{
					var binding = action.Bindings[j];

					GUI.Label( new Rect( 45, y, 300, y + h ), binding.DeviceName + ": " + binding.Name );
					if (GUI.Button( new Rect( 20, y + 3.0f, 20, h - 5.0f ), "-" ))
					{
						action.RemoveBinding( binding );
					}
					y += h;
				}

				if (GUI.Button( new Rect( 20, y + 3.0f, 20, h - 5.0f ), "+" ))
				{
					action.ListenForBinding();
				}

				if (GUI.Button( new Rect( 50, y + 3.0f, 50, h - 5.0f ), "Reset" ))
				{
					action.ResetBindings();
				}

				y += 25.0f;
			}

			if (GUI.Button( new Rect( 20, y + 3.0f, 50, h ), "Load" ))
			{
				LoadBindings();
			}

			if (GUI.Button( new Rect( 80, y + 3.0f, 50, h ), "Save" ))
			{
				SaveBindings();
			}

			if (GUI.Button( new Rect( 140, y + 3.0f, 50, h ), "Reset" ))
			{
				playerInput.Reset();
			}
		}
	}
}

