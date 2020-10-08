using UnityEngine;
using UnityEngine.UI;


public class StringToTextSetter : MonoBehaviour
{
    public Text Text;

    public StringVariable Variable;

    public bool AlwaysUpdate;

    private void OnEnable()
    {
        Text.text = Variable.value + "";
    }

    private void Update()
    {
        if (AlwaysUpdate && Variable)
        {
            Text.text = Variable.value + "";
        }
    }
}