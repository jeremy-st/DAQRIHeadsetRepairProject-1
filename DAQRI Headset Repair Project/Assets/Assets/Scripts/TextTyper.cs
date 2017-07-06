using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class TextTyper : MonoBehaviour, IPointerClickHandler
{

    public float letterPause = 0.2f;
    private IEnumerator coroutine;
    private IEnumerator coroutine2;

    string message = "";
    public Text textComp;

    // Use this for initialization

    public void OnPointerClick(PointerEventData eventData)
    {
        coroutine = TypeText();
        StartCoroutine(coroutine);
    }
    private IEnumerator TypeText()
    {
        message = "123 ";
        textComp.text = "";
        foreach (char letter in message.ToCharArray())
        {
                textComp.text = letter.ToString();
                yield return new WaitForSeconds(1);
        }
    }
}