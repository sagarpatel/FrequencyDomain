using UnityEngine;
using System.Collections;
using AmplifyColor;

[AddComponentMenu( "" )]
public class AmplifyColorVolumeBase : MonoBehaviour
{
	public Texture2D LutTexture;
	public float EnterBlendTime = 1.0f;	
	public int Priority = 0;
	public bool ShowInSceneView = true;

	[HideInInspector] public VolumeEffectContainer EffectContainer = new VolumeEffectContainer();

	void OnDrawGizmos()
	{
		if ( ShowInSceneView )
		{
			BoxCollider bc = GetComponent<BoxCollider>();
			if ( bc != null )
			{
				Gizmos.color = Color.green;
				Gizmos.DrawIcon( transform.position, "lut-volume.png", true );
				Gizmos.matrix = transform.localToWorldMatrix;
				Gizmos.DrawWireCube( bc.center, bc.size );
			}
		}
	}

	void OnDrawGizmosSelected()
	{
		BoxCollider bc = GetComponent<BoxCollider>();
		if ( bc != null )
		{
			Color col = Color.green;
			col.a = 0.2f;
			Gizmos.color = col;
			Gizmos.matrix = transform.localToWorldMatrix;
			Gizmos.DrawCube( bc.center, bc.size );
		}
	}
}
