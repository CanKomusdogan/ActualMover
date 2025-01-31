using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace ActualMover.Shared;

public static class ToastHelper
{
    public static async Task Show(string message, CancellationToken ct,
        ToastDuration toastDuration = ToastDuration.Long)
    {
        await Toast.Make(message, toastDuration).Show(ct);
    }

    public static async Task DelayedShow(string message, CancellationToken ct, TimeSpan delay,
        ToastDuration toastDuration = ToastDuration.Long)
    {
        await Task.Delay(delay, ct);
        await Toast.Make(message, toastDuration).Show(ct);
    }
}