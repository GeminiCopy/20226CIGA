using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DialogType
{
    Default,BlankScreen    
}

public struct Sentence
{
    public int Time;
    public string Content;
}

public class DialogManager : Singleton<DialogManager>
{
    public void Load()
    {
        
    }
    public void Play()
    {
        
    }
}

public class DialogController : MonoBehaviour
{
    
}