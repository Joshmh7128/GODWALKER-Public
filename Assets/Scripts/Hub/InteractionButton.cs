using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rewired;

public class InteractionButton : MonoBehaviour
{
    bool mouseIn = false;
    Player player;
    public bool canInteract = false;
    [SerializeField] Button ourButton;
    public Color idleColor;
    public Color hoverColor;
    public Color clickColor;
    [SerializeField] Image ourImage;
    [SerializeField] Text panelTitle;
    [SerializeField] Text panelDesc;
    [SerializeField] string customTitle;
    [SerializeField] string customDesc;

    private void Start()
    {
        player = ReInput.players.GetPlayer(0);
    }

    private void Update()
    {
        if (mouseIn)
        {
            ourImage.color = hoverColor;
        } else
        {
            ourImage.color = idleColor;
        }
        
        if (player.GetButtonDown("Fire"))
        {
            if (mouseIn == true && canInteract == true)
            {
                ourImage.color = clickColor;
                ourButton.onClick.Invoke();
            }
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Mouse"))
        {
            mouseIn = true;
            panelTitle.text = customTitle;
            panelDesc.text = customDesc;
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.CompareTag("Mouse"))
        {
            mouseIn = false;
            panelTitle.text = " ";
            panelDesc.text = " ";
        }
    }

    public void ButtonTestFunction(string text)
    {
        Debug.Log("DEBUG TEST FUNCTION: " + text);
    }
}
