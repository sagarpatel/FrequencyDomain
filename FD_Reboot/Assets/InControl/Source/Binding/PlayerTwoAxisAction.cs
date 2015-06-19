using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;


namespace InControl
{
	public class PlayerTwoAxisAction : TwoAxisInputControl
	{
		PlayerAction negativeXAction;
		PlayerAction positiveXAction;
		PlayerAction negativeYAction;
		PlayerAction positiveYAction;


		internal PlayerTwoAxisAction( PlayerAction negativeXAction, PlayerAction positiveXAction, PlayerAction negativeYAction, PlayerAction positiveYAction )
		{
			this.negativeXAction = negativeXAction;
			this.positiveXAction = positiveXAction;
			this.negativeYAction = negativeYAction;
			this.positiveYAction = positiveYAction;

			Raw = true;
		}


		internal void Update( ulong updateTick, float deltaTime )
		{
			var x = ValueFromSides( negativeXAction, positiveXAction );
			var y = ValueFromSides( negativeYAction, positiveYAction );
			UpdateWithAxes( x, y, updateTick, deltaTime );
		}


		float ValueFromSides( float negativeSideValue, float positiveSideValue )
		{
			var nsv = Mathf.Abs( negativeSideValue );
			var psv = Mathf.Abs( positiveSideValue );
			return nsv > psv ? -nsv : psv;
		}
	}
}
