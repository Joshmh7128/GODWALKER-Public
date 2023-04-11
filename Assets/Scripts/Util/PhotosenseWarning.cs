using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotosenseWarning : MonoBehaviour
{
    // script exists to show main menu once the photosense warning has been passed
    [SerializeField] GameObject mainMenu;
    // show the main menu
    public void ShowMenu() 
    { 
        mainMenu.SetActive(true); 
        gameObject.SetActive(false); 
    }
}
