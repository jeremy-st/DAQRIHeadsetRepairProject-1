using DAQRI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BlowupController : MonoBehaviour {

    [SerializeField]
    private float maxDistance = 20f;

    [SerializeField]
    private DisplayManager currentDisplayManager;

    [SerializeField]
    private GameObject mainCanvas;

    [SerializeField]
    private GameObject disabledCanvas;

    [SerializeField]//X and Y are screen cordinates on the camera and the z is used for depth/distance from camera forward.
    private Vector3 posAdjustments = new Vector3(0, 0, 0);

    private float initialTimeDelay = .1f;

    [SerializeField]
    public GameObject experienceMenu;
    [SerializeField]
    public bool FromMainMenu;
    private GameObject reticle;
    public List<Animator> AnimotorList;
    public static List<GameObject> UndoList = new List<GameObject>();
    [SerializeField]
    public GameObject MainMenu;
    [SerializeField]
    public GameObject Instructions;
    [SerializeField]
    public GameObject ResetPanel;
    [SerializeField]
    public GameObject UndoPanel;

    [HideInInspector]
    public bool value;
    [HideInInspector]
    public static bool RemoveStatus;

    public void SetPreviousScreen(bool set)
    {
        FromMainMenu = set;
    }
    public void GoToPreviousScreen()
    {
        ClearText();
        gameObject.SetActive(false);
        if (FromMainMenu)
            MainMenu.SetActive(true);
        else
            Instructions.SetActive(true);
    }
    public void Removecomponents()
    {
        ClearText();
        RemoveStatus = !RemoveStatus;
        var imgs = reticle.GetComponentsInChildren<Image>(true);
        if (RemoveStatus)
        {
            foreach (var img in imgs)
            {
                img.GetComponent<Image>().color = new Color32(255, 0, 0, 255);
            }
        }
        else
        {
            foreach (var img in imgs)
            {
                img.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            }
        }
    }

    public void ResetComponents(GameObject Remove)
    {
        ClearText();
        foreach (Animator ani in AnimotorList)
        {
            ani.gameObject.SetActive(true);
        }
        if (RemoveStatus)
        {
            var imgs = reticle.GetComponentsInChildren<Image>(true);
            foreach (var img in imgs)
            {
                img.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            }
            var on = Remove.transform.Find("Remove_On");
            on.gameObject.SetActive(true);
            var off = Remove.transform.Find("Remove_Off");
            off.gameObject.SetActive(false);

        }
        RemoveStatus = !RemoveStatus;
        ResetPanel.SetActive(false);
    }
    public void UndoRemovingComponent(GameObject Remove)
    {
        if (UndoList.Count > 0)
        {
            ClearText();
            var ob = UndoList[UndoList.Count - 1];
            ob.SetActive(true);
            UndoList.Remove(ob);
            if (UndoList.Count == 0)
            {
                var imgs = reticle.GetComponentsInChildren<Image>(true);
                foreach (var img in imgs)
                {
                    img.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                }
                var on = Remove.transform.Find("Remove_On");
                on.gameObject.SetActive(true);
                var off = Remove.transform.Find("Remove_Off");
                off.gameObject.SetActive(false);
                ResetPanel.SetActive(false);
                UndoPanel.SetActive(false);
                RemoveStatus = !RemoveStatus;
            }            
            
        }
    }
    private void ClearText()
    {
        try
        {
            EventSystem eventSystem = EventSystem.current;
            var RIM = (ReticleInputModule)eventSystem.gameObject.GetComponent(typeof(ReticleInputModule));
            GameObject tmp = GameObject.Find("Reticle Text");
            Text txt = tmp.GetComponent<Text>();
            txt.text = string.Empty;
        }
        catch { }
    }
    public void RepositionCanvas()
    {
        SwitchUI(false);
        AttachCanvas();
    }

    public void OnCountdownFinished()
    {
        DetachCanvas();
    }

    private void SwitchUI(bool activate)
    {
        reticle.SetActive(activate);
        experienceMenu.SetActive(activate);
    }

    private void Awake()
    {
        currentDisplayManager = DisplayManager.Instance;       
    }

    private IEnumerator Start()
    {
        Debug.Log(FromMainMenu);
        yield return new WaitForSeconds(initialTimeDelay);
        if (reticle == null)
        {
            reticle = FindObjectOfType<Reticle>().gameObject;
        }
        TransformSync();        
        AttachWtihoutTimer();
        DetachCanvas();  
    }

    private void Update()
    {
        ClampDistance();
        bool setRest = false;
        foreach (Animator ani in AnimotorList)
        {
            if(!ani.gameObject.active)
            { setRest = true; break; }
        }
        if (setRest)
        {
            ResetPanel.SetActive(true);
            UndoPanel.SetActive(true);
            experienceMenu.transform.localPosition = new Vector3(0.0184f, 0.143f, 1.932f);
        }
        else
        {
            ResetPanel.SetActive(false);
            UndoPanel.SetActive(false);
            experienceMenu.transform.localPosition = new Vector3(0.1155f, 0.143f, 1.932f);
            if (UndoList != null)
                UndoList.Clear();
        }
    }

    private void ClampDistance()
    {
        float currentDistance = Vector3.Distance(currentDisplayManager.transform.position, transform.position);

        if (currentDistance > maxDistance)
        {
            Vector3 updatedVector = currentDisplayManager.transform.position - transform.position;
            updatedVector = updatedVector.normalized;
            updatedVector *= currentDistance - maxDistance;
            transform.position += updatedVector;
        }
    }

    private void TransformSync()
    {
        Vector3 worldPoint =
            Camera.main.ViewportToWorldPoint(new Vector3(0.5f + posAdjustments.x,
                0.5f + posAdjustments.y,
                posAdjustments.z));
        transform.position = worldPoint;
        transform.eulerAngles = currentDisplayManager.transform.eulerAngles;
    }

    private void AttachCanvas()
    {
        disabledCanvas.SetActive(true);
        TransformSync();
        mainCanvas.transform.position = currentDisplayManager.transform.position + new Vector3(0, 10, 0);
        transform.SetParent(currentDisplayManager.transform, true);
    }
    private void AttachWtihoutTimer()
    {
        TransformSync();
        mainCanvas.transform.position = currentDisplayManager.transform.position + new Vector3(0, 10, 0);
        transform.SetParent(currentDisplayManager.transform, true);
    }

    private void DetachCanvas()
    {
        mainCanvas.transform.localPosition = Vector3.zero;
        transform.SetParent(null, true);
        TransformSync();
        SwitchUI(true);
    }  

    public void ToggleBool()
    {        
        value = !value; 
        foreach (Animator ani in AnimotorList)
        {
            ani.SetBool("Start", value);
        }
    }

}