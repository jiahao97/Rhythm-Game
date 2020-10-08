using UnityEngine;

[CreateAssetMenu]
public class StringVariable : ScriptableObject
{

    public string value;

    public void SetValue(string _value)
    {
        value = _value;
    }

    public void SetValue(StringVariable _value)
    {
        value = _value.value;
    }

    public void ApplyChange(string text)
    {
        value = text;
    }

    public void ApplyChange(StringVariable text)
    {
        value = text.value;
    }

}