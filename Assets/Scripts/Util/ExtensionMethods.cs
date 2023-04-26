using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
    static System.Random rng = new System.Random();

    // this might be too intensive...
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    // make transforms look at the player
    public static void LookAtPlayer(this Transform transform)
    {
        transform.LookAt(PlayerController.instance.transform.position);
    }

    // look at the main camera in the scene
    public static void LookAtCamera(this Transform transform)
    {
        transform.LookAt(Camera.main.transform);
    }

    // follow a target transform
    public static void FollowPos(this Transform transform, Transform target)
    {
        if (target && transform.position != target.position)
            transform.position = target.position;
    }

    // unparent a transform
    public static void Unparent(this Transform transform)
    {
        transform.parent = null;
    }

    // randomly return a random element of a list
    public static T RandomSelect<T>(this List<T> list)
    {
        return list[Random.Range(0, list.Count)];
    }

}
