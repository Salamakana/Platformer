using UnityEngine;
using System.Collections.Generic;

public class TimeController : Singelton<TimeController>
{
    public Transform PlayerTransform;
    public int RewindSeconds = 2;
    private Stack<Vector2> positions = new Stack<Vector2>();
    private bool isRewinding = false;

    private void Start()
    {
        PlayerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void StartRewind()
    {
        isRewinding = true;
    }

    public void StopRewind()
    {
        isRewinding = false;
    }

    private void Rewind()
    {
        if(positions.Count > 0)
        {
            PlayerTransform.position = positions.Pop();
        }
        else
        {
            StopRewind();
        }
    }

    private void Record()
    {
        if(positions.Count > Mathf.Round(RewindSeconds / Time.fixedDeltaTime))
        {
            positions.Clear();
        }

        positions.Push(PlayerTransform.position);
    }

    private void FixedUpdate()
    {
        if (isRewinding)
        {
            Rewind();
        }
        else
        {
            Record();
        }
    }
}
