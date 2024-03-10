using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public int currentTool = 0;
    public Action[] actions;

    public Transform startPos;

    public RectTransform toolbar;

    private void OnEnable()
    {
        actions[currentTool].startPos = startPos;
        actions[currentTool].enabled = true;
    }
    private void OnDisable()
    {
        actions[currentTool].enabled = false;
    }

    public void NextTool()
    {
        SwitchTool(currentTool + 1);
    }

    public void PreviousTool()
    {
        SwitchTool(currentTool - 1);
    }

    public void SwitchTool(int toolId)
    {
        actions[currentTool].image.enabled = false;
        if (toolId < 0)
        {
            toolId = actions.Length - 1;
        }
        else if (toolId >= actions.Length)
        {
            toolId = 0;
        }
        //actions[currentTool].enabled = false;
        //actions[currentTool].OnReset();
        currentTool = toolId;
        actions[currentTool].image.enabled = true;
        if (enabled)
        {
            actions[currentTool].enabled = true;
        }
    }

    public void AddAction(Action a)
    {
        toolbar.sizeDelta += new Vector2(55f, 0f);
        a.image.gameObject.SetActive(true);

        Action[] newActions = new Action[actions.Length + 1];
        for (int i = 0; i < actions.Length; i++)
        {
            newActions[i] = actions[i];
        }
        newActions[actions.Length] = a;
        actions = newActions;
    }
}
