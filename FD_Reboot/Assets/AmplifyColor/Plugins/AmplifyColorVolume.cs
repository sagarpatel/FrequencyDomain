using UnityEngine;
using System.Collections;

[RequireComponent( typeof( BoxCollider ) )]
[AddComponentMenu( "Image Effects/Amplify Color Volume" )]
public class AmplifyColorVolume : AmplifyColorVolumeBase
{	
	void OnTriggerEnter( Collider other )
	{
		AmplifyColorTriggerProxy tp = other.GetComponent<AmplifyColorTriggerProxy>();
		if ( tp != null && tp.OwnerEffect.UseVolumes && ( tp.OwnerEffect.VolumeCollisionMask & ( 1 << gameObject.layer ) ) != 0 )
			tp.OwnerEffect.EnterVolume( this );
	}

	void OnTriggerExit( Collider other )
	{
		AmplifyColorTriggerProxy tp = other.GetComponent<AmplifyColorTriggerProxy>();
		if ( tp != null && tp.OwnerEffect.UseVolumes && ( tp.OwnerEffect.VolumeCollisionMask & ( 1 << gameObject.layer ) ) != 0 )
			tp.OwnerEffect.ExitVolume( this );
	}
}
