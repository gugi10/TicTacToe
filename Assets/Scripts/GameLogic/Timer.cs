using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public static class StaticTimer
{
    private static CancellationTokenSource _cancellationTokenSource;

    public static async void StartTimer(int millisecondsDelay, Action onTimeoutCallback)
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = new CancellationTokenSource();
        try
        {
            await Task.Delay(millisecondsDelay, _cancellationTokenSource.Token);
            if (Application.isPlaying)
            {
                onTimeoutCallback?.Invoke();
            }
            else
            {
                _cancellationTokenSource?.Cancel();
            }
        }
        catch (TaskCanceledException)
        {
        }
    }
}