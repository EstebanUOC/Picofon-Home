public class BasicInput : InputField
{
    public int MinLength = 1;

    protected override void ValidateInput(string input)
    {
        Valid = input.Length >= MinLength;
    }
}
