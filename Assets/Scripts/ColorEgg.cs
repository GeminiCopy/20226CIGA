using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ColorEgg : MonoBehaviour
{
    public TMP_Text Title;
    public TMP_Text smallTitle;
    public Button Button1;
    public Button Button2;
    public Button Button3;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ShowEgg()
    {
        transform.gameObject.SetActive(true);
        Title.gameObject.SetActive(false);
        smallTitle.gameObject.SetActive(false);
    }
    public void SwitchImage0()
    {
        Button1.gameObject.SetActive(true);
    }
    public void SwitchImage1()
    {
        Button1.gameObject.SetActive(false);
        Button2.gameObject.SetActive(true);
    }
    public void SwitchImage2()
    {
        Button2.gameObject.SetActive(false);
        Button3.gameObject.SetActive(true);
    }
    public void SwitchImage3()
    {
        Button3.gameObject.SetActive(false);
        Button1.gameObject.SetActive(true);
    }
    public void CloseImage()
    {
        Button3.gameObject.SetActive(false);
        Button2.gameObject .SetActive(false);
        Button1 .gameObject .SetActive(false);
        Title.gameObject.SetActive(true);
        smallTitle.gameObject.SetActive(true);
    }
}
