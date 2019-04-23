using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using Tobii.Research.Unity;

public class SaveGazeDuration : MonoBehaviour
{
    #region Gaze Duration Properties

    // Makes it possible to store different gaze durations. KEY = gameObject.tag.
    private Dictionary<string, float> hashGazeDurations;

    // Used to retrieve the tag [key in the hashmap] when storing gaze duration.
    private GameObject latestHitObject;

    // Store the first object gaze time.
    private float startGazeTime = 0f;

    // A flag to know when the user is staring at the same object.
    private bool gazeTimeStarted = false;

    // Get the gaze information
    private VRGazeTrail gaze;

    // 
    private int stimuliLayer;
    

    #endregion Gaze Duration Properties

    #region Save Data Properties

    private XmlWriterSettings _fileSettings;
    private XmlWriter _file;
    [SerializeField]
    [Tooltip("Folder in the application root directory where data is saved.")]
    private string _folder = "Data";

    #endregion Save Data Properties


    // Start is called before the first frame update
    void Start()
    {
        stimuliLayer = LayerMask.NameToLayer("StimuliTarget");
        gaze = VRGazeTrail.Instance;
        hashGazeDurations = new Dictionary<string, float>();
        saveData();
    }

    // Update is called once per frame
    void Update()
    {
        if(gaze.LatestHit.transform != null)
        {
            GameObject currentHitObject = gaze.LatestHit.transform.gameObject;
            if (currentHitObject.layer == stimuliLayer)
            {
                onObjectGazeFixation(currentHitObject);
            }
            else
            {
                onObjectGazeFixationEnd();
            }
        }
    }

    public void onObjectGazeFixation(GameObject currentHitObject)
    {
        if (!gazeTimeStarted)
        {
            startGazeTime = Time.time;
            gazeTimeStarted = true;
            latestHitObject = currentHitObject;
        }
    }

    public void onObjectGazeFixationEnd()
    {
        if (gazeTimeStarted)
        {
            gazeTimeStarted = false;

            if (!hashGazeDurations.ContainsKey(latestHitObject.tag))
            {
                hashGazeDurations.Add(latestHitObject.tag, (Time.time - startGazeTime));
            }
            else
            {
                hashGazeDurations[latestHitObject.tag] += Time.time - startGazeTime;
            }

            Debug.Log("GazeDuration | object: " + latestHitObject.tag + " time:" + hashGazeDurations[latestHitObject.tag]);
        }
    }

    

    private void saveData()
    {
        if (_file == null)
        {
            // Opens data file. It becomes non-null.
            OpenDataFile();
        }
    }

    private void OpenDataFile()
    {
        if (_file != null)
        {
            Debug.Log("Already saving data.");
            return;
        }

        if (!System.IO.Directory.Exists(_folder))
        {
            System.IO.Directory.CreateDirectory(_folder);
        }

        _fileSettings = new XmlWriterSettings();
        _fileSettings.Indent = true;
        var fileName = string.Format("vr_data_{0}.xml", System.DateTime.Now.ToString("yyyyMMddTHHmmss"));
        _file = XmlWriter.Create(System.IO.Path.Combine(_folder, fileName), _fileSettings);
        _file.WriteStartDocument();
        _file.WriteStartElement("Data");
    }

    private void OnDestroy()
    {
        _file.WriteStartElement("GazeDurationData");
        foreach (KeyValuePair<string, float> entry in hashGazeDurations)
        {
            _file.WriteAttributeString(entry.Key, entry.Value.ToString());
        }
        _file.WriteEndElement();
        CloseDataFile();
    }


    private void CloseDataFile()
    {
        if (_file == null)
        {
            return;
        }

        _file.WriteEndElement();
        _file.WriteEndDocument();
        _file.Flush();
        _file.Close();
        _file = null;
        _fileSettings = null;
    }

}
