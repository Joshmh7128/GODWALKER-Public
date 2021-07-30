using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySphereBomb : MonoBehaviour
{
    // variables
    [SerializeField] GameObject cubePuff; // our particle effect
    Transform enemyManager;
    public PlayerController playerController; // player

    private void Start()
    {
        // add to list
        AddToManager();

        // set our parent
        enemyManager = GameObject.Find("Enemy Manager").transform;
        transform.SetParent(enemyManager);

        // start the safety kill
        StartCoroutine("SafetyKill");
    }

    public void AddToManager()
    {
        GameObject.Find("Enemy Manager").GetComponent<EnemyManager>().enemies.Add(gameObject);
    }

    IEnumerator SafetyKill()
    {
        yield return new WaitForSeconds(5f);
        Instantiate(cubePuff, transform.position, Quaternion.Euler(new Vector3(0, 0, 0)), null);
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider collision)
    {
        // if this hits the player
        if (collision.CompareTag("Player"))
        {
            playerController.AddHP(-1);
            Camera.main.GetComponent<CameraScript>().shakeDuration += 0.085f;
            Instantiate(cubePuff, transform.position, Quaternion.Euler(new Vector3(0, 0, 0)), null);
            Destroy(gameObject);
        }
        
        // if a bullet hits this
        if (collision.CompareTag("Projectile"))
        {
            Camera.main.GetComponent<CameraScript>().shakeDuration += 0.085f;
            Instantiate(cubePuff, transform.position, Quaternion.Euler(new Vector3(0, 0, 0)), null);
            Destroy(gameObject);
        }

        // if this hits a breakable
        if (collision.CompareTag("Breakable"))
        {
            // anything with the Breakable tag will be a chunk and have a BreakableBreak function
            collision.GetComponent<BreakableChunk>().BreakableBreak();
            Instantiate(cubePuff, transform.position, Quaternion.Euler(new Vector3(0, 0, 0)), null);
            Destroy(gameObject);
        }
    }
}
