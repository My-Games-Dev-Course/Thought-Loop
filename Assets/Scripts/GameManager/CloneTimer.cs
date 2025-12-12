using TMPro;
using UnityEngine;
using UnityEngine.Profiling;
/**
 * This code going to be for a the CloneTimer.
 */

public class CloneTimer : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private float startTime = 10f;
    [SerializeField] private GameObject clonePrefab;  // From the prefab folder
    [SerializeField] private CloneSpawner cloneSpawner;
    //[SerializeField] private GameObject player; // When we need to access the player for the recorder


    private float currentTime;
    private bool timerRunning = true;
    //private PlayerRecorder recorder; // Old version for the timer

    void Start()
    {
        currentTime = startTime;
    }

    void Update()
    {
        if (timerRunning && currentTime > 0)
        {
            currentTime -= Time.deltaTime;

            if (currentTime <= 0)
            {
                currentTime = 0;
                timerRunning = false;
                TimerEnded();
            }

            //timerText.text = Mathf.CeilToInt(currentTime).ToString();
            timerText.text = $"Clone in: {Mathf.CeilToInt(currentTime)}";
        }
    }

    void TimerEnded()
    {
        timerText.gameObject.SetActive(false);
        cloneSpawner.SpawnClone();
    }
}

//// Start is called once before the first execution of Update after the MonoBehaviour is created
//void Start()
//{
//    currentTime = startTime;
//    recorder = player.GetComponent<PlayerRecorder>();
//}

//// Update is called once per frame
//void Update()
//{
//    if (timerRunning && currentTime > 0)
//    {
//        currentTime -= Time.deltaTime;

//        if (currentTime <= 0)
//        {
//            currentTime = 0;
//            timerRunning = false;
//            TimerEnded();
//        }

//        // Show as integer (ceiling so 10.5 shows as 10, 9.1 shows as 9, etc.)
//        timerText.text = Mathf.CeilToInt(currentTime).ToString();
//    }
//}

//void TimerEnded()
//{
//    Debug.Log("Time's up! Clone is going to apear");
//    timerText.gameObject.SetActive(false);  // Hide the text
//    //SpawnClone();
//    cloneSpawner.SpawnClone();
//    Debug.Log("Clone has appeared!");
//}

//void SpawnClone()
//    {
//        // Stop recording
//        recorder.StopRecording();

//        // Get recorded data
//        var positions = recorder.GetPositions();
//        var rotations = recorder.GetRotations();
//        var scales = recorder.GetScales();
//        var flipXStates = recorder.GetFlipXStates();
//        var isRunningStates = recorder.GetIsRunningStates();

//        if (positions.Count > 0)
//        {
//            // Spawn clone at starting position
//            GameObject clone = Instantiate(clonePrefab, positions[0], rotations[0]);

//            // Start playback
//            ClonePlayback playback = clone.GetComponent<ClonePlayback>();
//            playback.StartPlayback(positions, rotations, scales, flipXStates, isRunningStates);

//            // Reset player to start (optional)
//            // player.transform.position = positions[0];
//        }
//    }
//}