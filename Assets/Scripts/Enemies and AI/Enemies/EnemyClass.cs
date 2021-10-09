using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class EnemyClass : MonoBehaviour
{
    public abstract void TakeDamage(int dmg, Vector3 dmgOrigin);
    public bool invincible = false; // are we invincible?
    public Slider HPslider; // our hp slider
    public Text HPTextAmount; // our hp slider
    public CanvasGroup HPcanvasGroup; // our canvas group

    public void AddToManager()
    {
        GameObject.Find("Enemy Manager").GetComponent<EnemyManager>().enemies.Add(gameObject);
    }
}
