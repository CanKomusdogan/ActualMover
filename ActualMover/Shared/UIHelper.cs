using System.Reflection;

namespace ActualMover.Shared;

public static class UIHelper
{
    public static void UpdateUI(dynamic uiEl, dynamic value, string? propertyName = null)
    {
        PropertyInfo? property = null;
        if (propertyName is not null)
        {
            property = uiEl.GetType().GetProperty(propertyName);
        }
        
        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (property is not null)
                property.SetValue(uiEl, value);
            else uiEl = value;
        });
    }
}