using System.Collections;
using UnityEngine;
using TMPro;

public class InteractiveReticle : MonoBehaviour
{

    public test test;
    public GameObject go;
    public Transform loc;
    public string text;
    private TMP_Text m_TextComponent;
    private bool iswait = false;
    void Start ()
    {

    }
 
    void Update ()
    {
        
    }
 
     public void OnPointerEnter()
    {
        go.SetActive(true);
        go.transform.position = loc.position;
        GameObject displaytext = GameObject.Find("displaytext");
        m_TextComponent = displaytext.GetComponent<TMP_Text>();
        go.transform.position += transform.up * 5f; 
        m_TextComponent.text = text;
        if(iswait==false){
            test.Speak(text);
            iswait = true;
        }
        StartCoroutine(waiter());
    }

    IEnumerator waiter()
    {
        yield return new WaitForSecondsRealtime(5);
        iswait = false;
    }
    
    
    // public void OnPointerHover(){
    //     // StartCoroutine(waiter());
    //     test.Speak(text);
    // }
    /// <summary>
    /// This method is called by the Main Camera when it stops gazing at this GameObject.
    /// </summary>
    public void OnPointerExit()
    {
       go.SetActive(false);
    }
    
 
}
 