using System;
using UnityEngine;

class Timer : MonoBehaviour
{
    public static Timer CreateInstance()
    {
        GameObject obj = new ("Timer");
        Timer timer = obj.AddComponent<Timer>();
        Instantiate(timer, Vector3.zero, Quaternion.identity);

        return timer;
    }

    public bool IsActive { get; private set; }

    private float _time = 0f;
    private float _duration = 0f;

    private Action _onTimerEnd;
    private bool _shouldDeactivateAfterTimerEnd = false;

    public void Init(float duration, Action onTimerEnd, bool deactivateAfterTimerEnd = false)
    {
        _duration = duration;
        _time = duration;

        _onTimerEnd = onTimerEnd;
        _shouldDeactivateAfterTimerEnd = deactivateAfterTimerEnd;
    }

    private void Update()
    {
        if (!IsActive) return;

        _time -= Time.deltaTime;

        if (_time <= 0)
        {
            _time = _duration;

            _onTimerEnd();

            if (_shouldDeactivateAfterTimerEnd) Deactivate();
        }
    }

    public void Activate() 
    {
        if (IsActive) return;
        IsActive = true;

        ResetTime();
    }

    public void Deactivate() 
    {
        if (!IsActive) return;
        IsActive = false;

        ResetTime();
    }

    public void ResetTime()
    {
        _time = _duration;
    }
}