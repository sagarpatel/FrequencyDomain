using System;
using UnityEngine;


namespace InControl
{
	/// <summary>
	/// This class represents a range inclusive of two values, and can remap a value from one range to another.
	/// </summary>
	public class InputRange
	{
		public static readonly InputRange MinusOneToOne = new InputRange( -1, 1 );
		public static readonly InputRange ZeroToOne = new InputRange( 0, 1 );
		public static readonly InputRange ZeroToMinusOne = new InputRange( 0, -1 );
		public static readonly InputRange ZeroToNegativeInfinity = new InputRange( 0, float.NegativeInfinity );
		public static readonly InputRange ZeroToPositiveInfinity = new InputRange( 0, float.PositiveInfinity );
		public static readonly InputRange Everything = new InputRange( float.NegativeInfinity, float.PositiveInfinity );

		/// <summary>
		/// The first value in the range.
		/// </summary>
		public float Value0;

		/// <summary>
		/// The second value in the range.
		/// </summary>
		public float Value1;


		/// <summary>
		/// Initializes an empty range.
		/// </summary>
		public InputRange()
		{
		}


		/// <summary>
		/// Initializes a new range from two given values.
		/// </summary>
		/// <param name="value0">The first value in the range.</param>
		/// <param name="value1">The second value in the range.</param>
		public InputRange( float value0, float value1 )
		{
			Value0 = value0;
			Value1 = value1;
		}


		/// <summary>
		/// Check whether a value falls within of this range.
		/// </summary>
		/// <returns><c>true</c>, if the value falls within this range, <c>false</c> otherwise.</returns>
		/// <param name="value">The value to check.</param>
		public bool Includes( float value )
		{
			return !Excludes( value );
		}


		/// <summary>
		/// Check whether a value falls outside of this range.
		/// </summary>
		/// <returns><c>true</c>, if the value falls outside this range, <c>false</c> otherwise.</returns>
		/// <param name="value">The value to check.</param>
		public bool Excludes( float value )
		{
			return (value < Mathf.Min( Value0, Value1 ) || value > Mathf.Max( Value0, Value1 ));
		}


		/// <summary>
		/// Remap the specified value, from one range to another.
		/// </summary>
		/// <param name="value">The value to remap.</param>
		/// <param name="sourceRange">The source range to map from.</param>
		/// <param name="targetRange">The target range to map to.</param>
		public static float Remap( float value, InputRange sourceRange, InputRange targetRange )
		{
			if (sourceRange.Excludes( value ))
			{
				return 0.0f;
			}
			var sourceValue = Mathf.InverseLerp( sourceRange.Value0, sourceRange.Value1, value );
			return Mathf.Lerp( targetRange.Value0, targetRange.Value1, sourceValue ); 
		}
	}
}
