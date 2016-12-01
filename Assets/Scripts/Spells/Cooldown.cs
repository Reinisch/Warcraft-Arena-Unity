using System;
using UnityEngine;

[Serializable]
public class Cooldown
{
    public float duration;
    public float timeLeft;

    public bool HasCooldown
    {
        get { return timeLeft > 0; }
    }

    public Cooldown(float seconds)
    {
        duration = seconds;
        timeLeft = 0;
    }

    public void Update()
    {
        if (!HasCooldown)
            return;

        timeLeft -= Time.deltaTime;
        if (timeLeft < 0)
            timeLeft = 0;
    }

    public void Apply()
    {
        if (duration == 0)
            return;
        timeLeft = duration;
    }
    public void ApplyModified(float haste)
    {
        if (duration == 0)
            return;
        timeLeft = duration/(1+haste/100);
    }

    public void Dispose()
    {

    }
}
