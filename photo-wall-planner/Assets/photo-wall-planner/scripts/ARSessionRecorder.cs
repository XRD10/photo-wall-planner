using UnityEngine;
using UnityEngine.XR.ARFoundation;
using System.Collections.Generic;
using System.IO;

public class ARSessionRecorder : MonoBehaviour
{
    public ARCameraManager arCameraManager;
    private List<ARFrameData> recordedFrames = new List<ARFrameData>();

    void Start()
    {
        arCameraManager.frameReceived += OnFrameReceived;
    }

    void OnFrameReceived(ARCameraFrameEventArgs args)
    {
        var frameData = new ARFrameData
        {
            timestamp = Time.time,
            position = arCameraManager.transform.position,
            rotation = arCameraManager.transform.rotation
        };

        recordedFrames.Add(frameData);
    }

    public void SaveRecording()
    {
        string path = Path.Combine(Application.persistentDataPath, "ARSessionRecording.json");
        File.WriteAllText(path, JsonUtility.ToJson(new ARSessionRecording { frames = recordedFrames }));
        Debug.Log($"Recording saved to {path}");
    }
}

[System.Serializable]
public class ARSessionRecording
{
    public List<ARFrameData> frames;
}

[System.Serializable]
public class ARFrameData
{
    public float timestamp;
    public Vector3 position;
    public Quaternion rotation;
}
