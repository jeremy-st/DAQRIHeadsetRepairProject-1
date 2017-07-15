using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace DAQRI {

    [ExecuteInEditMode]
    [RequireComponent (typeof (Text))]
    public class TextCapitalization : MonoBehaviour {

        private Text textComponent;

        void Start () {
            textComponent =  GetComponent<Text> ();
        }

        void Update () {
            textComponent.text = textComponent.text.ToUpper ();
        }
    }
}
