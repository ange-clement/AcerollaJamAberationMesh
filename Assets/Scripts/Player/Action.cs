using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Action : MonoBehaviour
{
    public Transform startPos;

    public Image image;

    public abstract void OnReset();
}
