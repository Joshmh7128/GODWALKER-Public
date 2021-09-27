using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rewired;

public class InteractionButton : MonoBehaviour
{
    bool mouseIn;
    Player player;
    [SerializeField] Button ourButton;

    private void Start()
    {
        player = ReInput.players.GetPlayer(0);
    }

    private void FixedUpdate()
    {
        if (player.GetButtonDown("Fire"))
        {
            ourButton.onClick.Invoke();
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Mouse"))
        {
            mouseIn = true;
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.CompareTag("Mouse"))
        {
            mouseIn = false;
        }
    }

    public void ButtonTestFunction(string text)
    {
        Debug.Log("DEBUG TEST FUNCTION: " + text);
    }
}
