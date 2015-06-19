// Amplify Color - Advanced Color Grading for Unity Pro
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace AmplifyColor
{
	public class BackBufferHandler
	{
		private readonly CameraCollection _cameras;

		public BackBufferHandler( CameraCollection cameras )
		{
			_cameras = cameras;
		}

		public bool ReadBackBuffer( out ImageResult imageResult )
		{
			imageResult = null;

			if ( _cameras == null )
			{
				Debug.LogError( "[AmplifyColor] Camera collection is invalid." );
				return false;
			}

			var camera = _cameras.SelectedCamera;

			if ( camera == null )
			{
				Debug.LogError( "[AmplifyColor] Selected camera is invalid." );
				return false;
			}

            var component = ( MonoBehaviour ) camera.GetComponent<AmplifyColorBase>();
			bool enabled = false;

			if ( !ToolSettings.Instance.ApplyLUT )
			{
				if ( component != null )
				{
					enabled = component.enabled;
					component.enabled = false;
				}
			}

			var width = ToolSettings.Instance.Resolution.TargetWidth;
			var height = ToolSettings.Instance.Resolution.TargetHeight;

			//if (ToolSettings.Instance.Resolution.IsGameWindowSize)
			//{
			//    width = Screen.width;
			//    height = Screen.height;
			//}

			var cameratarget = camera.targetTexture;

			var rt = RenderTexture.GetTemporary( width, height, 24, RenderTextureFormat.ARGB32 );
			camera.targetTexture = rt;
			camera.Render();
			camera.targetTexture = cameratarget;

			var activert = RenderTexture.active;
			RenderTexture.active = rt;
			var text = new Texture2D( width, height, TextureFormat.ARGB32, false );
			text.ReadPixels( new Rect( 0, 0, width, height ), 0, 0 );
			text.Apply();
			RenderTexture.active = activert;
			var colors = text.GetPixels( 0, 0, width, height );

			var colordata = new Color[ width, height ];

			for ( int i = height - 1; i >= 0; i-- )
			{
				for ( int j = 0; j < width; j++ )
				{
					colordata[ j, ( height - 1 - i ) ] = colors[ i * width + j ];
					colordata[ j, ( height - 1 - i ) ].a = 1;
				}
			}

			if ( !ToolSettings.Instance.ApplyLUT )
			{
				if ( component != null )
					component.enabled = enabled;
			}

			imageResult = new ImageResult( colordata );

			return true;
		}
	}
}