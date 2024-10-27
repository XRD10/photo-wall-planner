using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;

public class ARSessionPlayback : MonoBehaviour
{
    public Camera arPlaybackCamera;
    [SerializeField]
    private GameObject playbackRecordingObject;
    [SerializeField]
    private Button stopButton;
    private ARSessionRecording playbackRecording;
    private int currentFrameIndex = 0;
    private float startTime;
    private bool isPlaying = true;


    void Start()
    {
        playbackRecordingObject.SetActive(false);
        stopButton.enabled = false;

        LoadRecording("ARSessionRecording.json");
        StartPlayback();
    }

    void Update()
    {
        if (isPlaying && currentFrameIndex < playbackRecording.frames.Count)
        {
            var frame = playbackRecording.frames[currentFrameIndex];

            if (Time.time - startTime >= frame.timestamp)
            {
                arPlaybackCamera.transform.position = frame.position;
                arPlaybackCamera.transform.rotation = frame.rotation;
                currentFrameIndex++;
            }

            if (currentFrameIndex >= playbackRecording.frames.Count)
            {
                isPlaying = false;
            }
        }
    }

    public void StartPlayback()
    {
        currentFrameIndex = 0;
        startTime = Time.time;
        isPlaying = true;
    }

    public void LoadRecording(string fileName)
    {
        string path = Path.Combine(Application.persistentDataPath, fileName);
        playbackRecording = JsonUtility.FromJson<ARSessionRecording>(File.ReadAllText(path));

        if (playbackRecording == null)
            Debug.LogError("Failed to load recording at: " + path);
    }

    public void ResetPlayback()
    {
        StartPlayback(); 
    }
}
