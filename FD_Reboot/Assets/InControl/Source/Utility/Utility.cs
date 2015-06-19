using System.Collections;
using System.IO;
using UnityEngine;


namespace InControl
{
	public static class Utility
	{
		private static Vector2[] circleVertexList = {
			new Vector2( +0.0000f, +1.0000f ),
			new Vector2( +0.2588f, +0.9659f ),
			new Vector2( +0.5000f, +0.8660f ),
			new Vector2( +0.7071f, +0.7071f ),
			new Vector2( +0.8660f, +0.5000f ),
			new Vector2( +0.9659f, +0.2588f ),
			new Vector2( +1.0000f, +0.0000f ),
			new Vector2( +0.9659f, -0.2588f ),
			new Vector2( +0.8660f, -0.5000f ),
			new Vector2( +0.7071f, -0.7071f ),
			new Vector2( +0.5000f, -0.8660f ),
			new Vector2( +0.2588f, -0.9659f ),
			new Vector2( +0.0000f, -1.0000f ),
			new Vector2( -0.2588f, -0.9659f ),
			new Vector2( -0.5000f, -0.8660f ),
			new Vector2( -0.7071f, -0.7071f ),
			new Vector2( -0.8660f, -0.5000f ),
			new Vector2( -0.9659f, -0.2588f ),
			new Vector2( -1.0000f, -0.0000f ),
			new Vector2( -0.9659f, +0.2588f ),
			new Vector2( -0.8660f, +0.5000f ),
			new Vector2( -0.7071f, +0.7071f ),
			new Vector2( -0.5000f, +0.8660f ),
			new Vector2( -0.2588f, +0.9659f ),
			new Vector2( +0.0000f, +1.0000f )			
		};


		public static void DrawCircleGizmo( Vector2 center, float radius )
		{
			var p = (circleVertexList[0] * radius) + center;
			var c = circleVertexList.Length;
			for (int i = 1; i < c; i++)
			{ 
				Gizmos.DrawLine( p, p = (circleVertexList[i] * radius) + center );
			}
		}


		public static void DrawCircleGizmo( Vector2 center, float radius, Color color )
		{
			Gizmos.color = color;
			DrawCircleGizmo( center, radius );
		}


		public static void DrawOvalGizmo( Vector2 center, Vector2 size )
		{
			var r = size / 2.0f;
			var p = Vector2.Scale( circleVertexList[0], r ) + center;
			var c = circleVertexList.Length;
			for (int i = 1; i < c; i++)
			{ 
				Gizmos.DrawLine( p, p = Vector2.Scale( circleVertexList[i], r ) + center );
			}
		}


		public static void DrawOvalGizmo( Vector2 center, Vector2 size, Color color )
		{
			Gizmos.color = color;
			DrawOvalGizmo( center, size );
		}


		public static void DrawRectGizmo( Rect rect )
		{
			var p0 = new Vector3( rect.xMin, rect.yMin );
			var p1 = new Vector3( rect.xMax, rect.yMin );
			var p2 = new Vector3( rect.xMax, rect.yMax );
			var p3 = new Vector3( rect.xMin, rect.yMax );
			Gizmos.DrawLine( p0, p1 );
			Gizmos.DrawLine( p1, p2 );
			Gizmos.DrawLine( p2, p3 );
			Gizmos.DrawLine( p3, p0 );
		}


		public static void DrawRectGizmo( Rect rect, Color color )
		{
			Gizmos.color = color;
			DrawRectGizmo( rect );
		}


		public static void DrawRectGizmo( Vector2 center, Vector2 size )
		{
			var hw = size.x / 2.0f;
			var hh = size.y / 2.0f;
			var p0 = new Vector3( center.x - hw, center.y - hh );
			var p1 = new Vector3( center.x + hw, center.y - hh );
			var p2 = new Vector3( center.x + hw, center.y + hh );
			var p3 = new Vector3( center.x - hw, center.y + hh );
			Gizmos.DrawLine( p0, p1 );
			Gizmos.DrawLine( p1, p2 );
			Gizmos.DrawLine( p2, p3 );
			Gizmos.DrawLine( p3, p0 );
		}


		public static void DrawRectGizmo( Vector2 center, Vector2 size, Color color )
		{
			Gizmos.color = color;
			DrawRectGizmo( center, size );
		}


		public static bool GameObjectIsCulledOnCurrentCamera( GameObject gameObject )
		{
			return (Camera.current.cullingMask & (1 << gameObject.layer)) == 0;
		}


		public static Color MoveColorTowards( Color color0, Color color1, float maxDelta )
		{
			var r = Mathf.MoveTowards( color0.r, color1.r, maxDelta );
			var g = Mathf.MoveTowards( color0.g, color1.g, maxDelta );
			var b = Mathf.MoveTowards( color0.b, color1.b, maxDelta );
			var a = Mathf.MoveTowards( color0.a, color1.a, maxDelta );
			return new Color( r, g, b, a );
		}


		public static float ApplyDeadZone( float value, float lowerDeadZone, float upperDeadZone )
		{
			return Mathf.InverseLerp( lowerDeadZone, upperDeadZone, Mathf.Abs( value ) ) * Mathf.Sign( value );
		}


		public static Vector2 ApplyCircularDeadZone( Vector2 v, float lowerDeadZone, float upperDeadZone )
		{
			var magnitude = Mathf.InverseLerp( lowerDeadZone, upperDeadZone, v.magnitude );
			return v.normalized * magnitude;
		}


		public static Vector2 ApplyCircularDeadZone( float x, float y, float lowerDeadZone, float upperDeadZone )
		{
			return ApplyCircularDeadZone( new Vector2( x, y ), lowerDeadZone, upperDeadZone );
		}


		public static float ApplySmoothing( float thisValue, float lastValue, float deltaTime, float sensitivity )
		{
			// 1.0f and above is instant (no smoothing).
			if (Mathf.Approximately( sensitivity, 1.0f ))
			{
				return thisValue;
			}

			// Apply sensitivity (how quickly the value adapts to changes).
			var maxDelta = deltaTime * sensitivity * 100.0f;

			// Snap to zero when changing direction quickly.
			if (Mathf.Sign( lastValue ) != Mathf.Sign( thisValue ))
			{
				lastValue = 0.0f;
			}

			return Mathf.MoveTowards( lastValue, thisValue, maxDelta );
		}


		internal static bool TargetIsButton( InputControlType target )
		{
			return (target >= InputControlType.Action1 && target <= InputControlType.Action4) || (target >= InputControlType.Button0 && target <= InputControlType.Button19);
		}


		internal static bool TargetIsStandard( InputControlType target )
		{
			return target >= InputControlType.LeftStickUp && target <= InputControlType.RightBumper;
		}


		public static string ReadFromFile( string path )
		{			
			var streamReader = new StreamReader( path );
			var data = streamReader.ReadToEnd();
			streamReader.Close();
			return data;
		}


		public static void WriteToFile( string path, string data )
		{
			var streamWriter = new StreamWriter( path );
			streamWriter.Write( data );
			streamWriter.Flush();
			streamWriter.Close();
		}


		public static bool Approximately( float value1, float value2 )
		{
			var delta = value1 - value2;
			return (delta >= -float.Epsilon) && (delta <= float.Epsilon);
		}


		public static bool IsNotZero( float value )
		{
			return (value < -float.Epsilon) || (value > float.Epsilon);
		}


		public static bool IsZero( float value )
		{
			return (value >= -float.Epsilon) || (value <= float.Epsilon);
		}


		public static bool AbsoluteIsOverThreshold( float value, float threshold )
		{
			return (value < -threshold) || (value > threshold);
		}
	}
}



