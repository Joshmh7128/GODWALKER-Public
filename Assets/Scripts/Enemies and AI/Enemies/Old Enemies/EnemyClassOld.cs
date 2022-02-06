using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class EnemyClassOld : MonoBehaviour
{
    public abstract void TakeDamage(int dmg);
    public bool invincible = false; // are we invincible?
    public Slider HPslider; // our hp slider
    public Text HPTextAmount; // our hp slider
    public CanvasGroup HPcanvasGroup; // our canvas group
    public RoomClass roomClass; // our roomclass

    public void AddToManager()
    {
        if (roomClass == null)
        { 
            roomClass = GetComponentInParent<EnemyClassOld>().roomClass; 
        }

        if (roomClass != null)
        {
            roomClass.enemyClasses.Add(this);
        }

        // so we can test externally
        if (GameObject.Find("Enemy Manager"))
        {
            GameObject.Find("Enemy Manager").GetComponent<EnemyManager>().enemies.Add(gameObject);
        }

        // once we are added remove our parent
        // transform.parent = null;
    }
}
