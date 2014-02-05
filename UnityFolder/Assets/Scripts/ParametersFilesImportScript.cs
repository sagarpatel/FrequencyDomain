using UnityEngine;
using System.Collections;
using System.IO;

public class ParametersFilesImportScript : MonoBehaviour 
{

	  // From: http://wiki.unity3d.com/index.php?title=ImprovedFileBrowser

    protected FileBrowser m_fileBrowser;
    protected Texture2D m_directoryImage, m_fileImage;

	string filePathtxt;

	public bool isActive = true;

	string lastUsedDirectorytxt = null;
	string mostRecentExtension = null;

	AudioDirectorScript audioDirector;
	AudioListener audioListener;
	GeneralEditorScript genralEditorScript;
	LiveAudioInputSelectorScript liveAudioInputSelector;
	CameraTypeSelectorScript cameraTypeSelector;

	// Use this for initialization
	void Start () 
	{
		audioDirector = (AudioDirectorScript) GameObject.FindWithTag("AudioDirector").GetComponent("AudioDirectorScript");
		audioListener = (AudioListener) GameObject.FindWithTag("AudioDirector").GetComponent("AudioListener");
		genralEditorScript = (GeneralEditorScript)GetComponent("GeneralEditorScript");
		liveAudioInputSelector = GetComponent<LiveAudioInputSelectorScript>();
		cameraTypeSelector = GetComponent<CameraTypeSelectorScript>();

		if(isActive)
		{
			Screen.showCursor = true;
			Screen.lockCursor = false;
		}
	}
	
	// Update is called once per frame
	void Update () 
	{

		// check input to activate button
		if( Input.GetButtonDown("Display Music Browser Button") == true )
		{
			if(isActive)
			{
				isActive = false;

				liveAudioInputSelector.isActive = false;
				cameraTypeSelector.isActive = false;
				Screen.showCursor = false;
				Screen.lockCursor = true;
			}
			else
			{
				isActive = true;
				genralEditorScript.isActive = false;

				liveAudioInputSelector.isActive = true;
				cameraTypeSelector.isActive = true;

				Screen.showCursor = true;
				Screen.lockCursor = false;
			}
		}
	
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

        // track parameters file broswing
        GUILayout.BeginHorizontal();
        GUILayout.Label("Press button to select a parameters file", GUILayout.Width(100));
        GUILayout.FlexibleSpace();
        GUILayout.Label(filePathtxt ?? "none selected");
        if( GUILayout.Button("Browse Parameters File!", GUILayout.ExpandWidth(false)) ) 
        {
            m_fileBrowser = new FileBrowser( new Rect(100, 100, 600, 500), "Choose Parameters File", FileSelectedCallback, lastUsedDirectorytxt);
            m_fileBrowser.SelectionPattern = "*.txt";
            m_fileBrowser.DirectoryImage = m_directoryImage;
            m_fileBrowser.FileImage = m_fileImage;
            mostRecentExtension = "txt";
        }
        GUILayout.EndHorizontal();

    }
 
    protected void FileSelectedCallback(string path, string directory) 
    {
        m_fileBrowser = null;
       
    	lastUsedDirectorytxt = directory;
    	filePathtxt = path;

    	if(path != null)
	 	{

	 	 	// load parameters text file into string
	    	string rawFileString = File.ReadAllText(filePathtxt);
	    	string[] rawSplit = rawFileString.Split('|');
	    	string amplitudeListString = rawSplit[1];
	    	string frequencyStartString = rawSplit[2];
	    	string frequencyListString = rawSplit[3];
	    	string rgbColorScaleFactorsString = rawSplit[4];


	    	// copy over amplitudes
	    	for(int i = 0; i < audioDirector.scalingPerDecadeArray.Length; i++)
	    		audioDirector.scalingPerDecadeArray[i] = float.Parse( amplitudeListString.Split(',')[i] );
	 		
	 		// copy over frequency start index
	 		audioDirector.sampleStartIndex = int.Parse( frequencyStartString.Trim() );

	 		// copy over frequency distribution
	 		for(int i = 0; i < audioDirector.samplesPerDecadeArray.Length; i++)
	 			audioDirector.samplesPerDecadeArray[i] = int.Parse( frequencyListString.Split(',')[i] );

	 		// copy over rgb scale values
	 		audioDirector.rScale = float.Parse( rgbColorScaleFactorsString.Split(',')[0] );
	 		audioDirector.gScale = float.Parse( rgbColorScaleFactorsString.Split(',')[1] );
	 		audioDirector.bScale = float.Parse( rgbColorScaleFactorsString.Split(',')[2] );
	 	}
	 
        //isActive = false;       
    }


}
