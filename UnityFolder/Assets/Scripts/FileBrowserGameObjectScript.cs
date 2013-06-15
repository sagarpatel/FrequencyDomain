using UnityEngine;
using System.Collections;

public class FileBrowserGameObjectScript : MonoBehaviour 
{

    // From: http://wiki.unity3d.com/index.php?title=ImprovedFileBrowser

    protected FileBrowser m_fileBrowser;
    protected Texture2D m_directoryImage, m_fileImage;

	public string filePath;
	public bool isActive = true;

	MP3Import mp3Importer;
	AudioDirectorScript audioDirector;

	// Use this for initialization
	void Start () 
	{
		mp3Importer = (MP3Import)GetComponent("MP3Import");
		audioDirector = (AudioDirectorScript) GameObject.Find("AudioDirector").GetComponent("AudioDirectorScript");
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}


    protected void OnGUI () 
    {
    	if(isActive)
    	{
	        if (m_fileBrowser != null) 
	        {
	            m_fileBrowser.OnGUI();
	        } 
	        else 
	        {
	            OnGUIMain();
	        }
	    }
    }
 
    protected void OnGUIMain() 
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Press button to select a mp3 file", GUILayout.Width(100));
        GUILayout.FlexibleSpace();
        GUILayout.Label(filePath ?? "none selected");
        if( GUILayout.Button("Browse Button!", GUILayout.ExpandWidth(false)) ) 
        {
            m_fileBrowser = new FileBrowser( new Rect(100, 100, 600, 500), "Choose Music (mp3) File", FileSelectedCallback);
            m_fileBrowser.SelectionPattern = "*.mp3";
            m_fileBrowser.DirectoryImage = m_directoryImage;
            m_fileBrowser.FileImage = m_fileImage;
        }
        GUILayout.EndHorizontal();
    }
 
    protected void FileSelectedCallback(string path) 
    {
        m_fileBrowser = null;
        filePath = path;

        //if() CHECK FOR FILE IS VALID
        //THEN
        isActive = false;
        mp3Importer.StartImport(filePath);

        audioDirector.audioSourceArray[0] = mp3Importer.audioSource;
        audioDirector.audioSourceArray[0].Play();
    }


}
