using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class InteractionMouse : MonoBehaviour
{
    // rewired player input
    Player player;
    float moveV;
    float moveH;
    RectTransform rectTransform;
    public bool canMove = false;
    [SerializeField] float xMax;
    [SerializeField] float yMax;

    // Start is called before the first frame update
    void Start()
    {
        // get our plaer
        player = ReInput.players.GetPlayer(0);
        rectTransform = gameObject.GetComponent<RectTransform>();
    }

    private void FixedUpdate()
    {
        if (canMove)
        {
            // get our movement
            moveV = player.GetAxis("MouseVertical");
            moveH = player.GetAxis("MouseHorizontal");
            rectTransform.localPosition = Vector2.Lerp(rectTransform.localPosition, new Vector2(Mathf.Clamp(rectTransform.localPosition.x+moveH, -xMax, xMax), Mathf.Clamp(rectTransform.localPosition.y+moveV, -yMax, yMax)), 1f);
        }
    }
}
