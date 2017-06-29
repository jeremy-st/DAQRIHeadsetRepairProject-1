using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TextTyper : MonoBehaviour
{

    public float letterPause = 0.2f;
    private IEnumerator coroutine;

    string message = "";
    public Text textComp;

    // Use this for initialization
    IEnumerator Start()
    {
        
        //yield return StartCoroutine(TypeText());
        coroutine = TypeText();
        yield return StartCoroutine(coroutine);
        
    }


    private IEnumerator TypeText()
    {
        message = "123";
        textComp.text = "";
        foreach (char letter in message.ToCharArray())
        {
            textComp.text = letter.ToString();
            Debug.Log(message);
            Debug.Log(letter);
            yield return new WaitForSeconds(5);
            Debug.Log("0");
        }
    }
}