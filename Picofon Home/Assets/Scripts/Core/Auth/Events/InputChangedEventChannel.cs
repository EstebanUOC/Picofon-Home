using System;

public class InputChangedEventChannel
{
    public event Action<bool> OnInputChanged;

    public void Raise(bool valid)
    {
        OnInputChanged?.Invoke(valid);
    }
}
