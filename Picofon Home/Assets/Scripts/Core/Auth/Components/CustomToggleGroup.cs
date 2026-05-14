using UnityEngine;

public class CustomToggleGroup : MonoBehaviour
{
    private bool _someSelected = false;

    private CustomToggle _selectedToggle;

    public void SelectToggle(CustomToggle toggle)
    {
        if (toggle == _selectedToggle)
        {
            return;
        }

        if (_someSelected)
        {
            _selectedToggle.ToggleOff();
            _selectedToggle = toggle;
            return;
        }

        _someSelected = true;
        _selectedToggle = toggle;
    }

    public int GetSelectedIndex()
    {
        return _selectedToggle.Index;
    }

    public bool ShouldToggle(CustomToggle toggle)
    {
        return toggle != _selectedToggle;
    }
}
