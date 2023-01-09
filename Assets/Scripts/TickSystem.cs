using System;
using UnityEngine;

public class TickSystem : MonoBehaviour
{
    public class OnTickEventArgs : EventArgs
    {
        public int tick;
    }
    public static event EventHandler<OnTickEventArgs> OnTick;

    private const float TICKS_PER_SEC = 30;

    private int tick;
    private float tickTimer;
    
    private void Awake()
    {
        tick = 0;
    }

    private void Update()
    {
        tickTimer += Time.deltaTime;
        if (tickTimer >= 1 / TICKS_PER_SEC)
        {
            tickTimer -= 1 / TICKS_PER_SEC;
            tick++;

            OnTickEvent();
        }
    }

    protected virtual void OnTickEvent()
    {
        OnTick?.Invoke(this, new OnTickEventArgs { tick = tick });
    }
}
