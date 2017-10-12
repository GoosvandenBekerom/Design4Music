using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class MagnetSensor : MonoBehaviour
{
    public delegate void CardboardTrigger();
    public static event CardboardTrigger OnCardboardTrigger;

    private const int FRAME_COUNT = 40;
    private const int TRIGGER_THRESHOLD = 90;

    private bool triggered;
    
    private Queue<float> _magnitudes;

    void Awake()
    {
        _magnitudes = new Queue<float>(FRAME_COUNT);
        triggered = false;
    }

    void OnEnable()
    {
        _magnitudes.Clear();
        Input.compass.enabled = true;
    }

    void OnDisable()
    {
        Input.compass.enabled = false;
    }

    void Update()
    {
        Vector3 currentVector = Input.compass.rawVector;

        if (_magnitudes.Count >= FRAME_COUNT) _magnitudes.Dequeue();
        _magnitudes.Enqueue(currentVector.magnitude);
        
        var diff = Mathf.Abs(_magnitudes.Average() - currentVector.magnitude);
        if ((diff >= TRIGGER_THRESHOLD && !triggered) || Input.GetMouseButtonDown(0))
        {
            triggered = true;
            OnCardboardTrigger();
        }
        else if (triggered && diff < TRIGGER_THRESHOLD)
        {
            triggered = false;
        }
    }
}