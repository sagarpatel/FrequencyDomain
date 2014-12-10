using UnityEngine;
using System.Collections;

public class PerspectiveTransition : MonoBehaviour 
{

	private Matrix4x4 orthoMatrix;
	private Matrix4x4 perspectiveMatrix;
	Camera mainCamera;

	public PVA pva;
	public float velocityThreashold = 100.0f;

	// using camera perspective ortho animation from  http://forum.unity3d.com/threads/32765-Smooth-transition-between-perspective-and-orthographic-modes

	void Start () 
	{
		mainCamera = GetComponent<Camera>();
		UpdateCameraMatrices();
	}
	
	// Update is called once per frame
	void Update () 
	{

		UpdateCameraMatrices();
		if(pva.velocityMagnitude < velocityThreashold )
		{
			LerpFromTo(orthoMatrix, perspectiveMatrix, (pva.velocityMagnitude *0.1f ) / velocityThreashold);
		}
		else
			mainCamera.projectionMatrix = perspectiveMatrix;

	}


	void UpdateCameraMatrices()
    {
    	float orthographicSize = mainCamera.orthographicSize;
    	float aspect = mainCamera.aspect;
    	float fieldOfView = mainCamera.fieldOfView;
        float nearClipPlane = mainCamera.nearClipPlane;
        float farClipPlane = mainCamera.farClipPlane;

        orthoMatrix = Matrix4x4.Ortho(-orthographicSize * aspect, orthographicSize * aspect, -orthographicSize, orthographicSize, nearClipPlane, farClipPlane);
        perspectiveMatrix = Matrix4x4.Perspective(fieldOfView, aspect, nearClipPlane, farClipPlane);
    }


    public static Matrix4x4 MatrixLerp(Matrix4x4 src, Matrix4x4 dest, float time)

    {
        Matrix4x4 ret = new Matrix4x4();
        for (int i = 0; i < 16; i++)
            ret[i] = Mathf.Lerp(src[i], dest[i], time);
        return ret;
    }

 

    private void LerpFromTo(Matrix4x4 src, Matrix4x4 dest, float step)

    {

        mainCamera.projectionMatrix = MatrixLerp(src, dest, step );
      
    }
}
