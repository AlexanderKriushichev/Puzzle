using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SpaceMan : MonoBehaviour {

    [TextArea]
    public string startText;
    [TextArea]
    public string endSuccessedText;
    [TextArea]
    public string endUnsuccessedText;

    public Text textMesh;

    void Start()
    {
        textMesh.text = startText;
    }

    public void SetSuccessedText()
    {
        textMesh.text = endSuccessedText;
    }

    public void SetUnsuccessedText()
    {
        textMesh.text = endUnsuccessedText;
    }
}
