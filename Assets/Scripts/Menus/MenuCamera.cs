using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCamera : MonoBehaviour
{
    //camera positions for each part of the menu are saved here
    [SerializeField] private Vector3 mainMenuPosition;
    [SerializeField] private Vector3 mainMenuRotation;
    [SerializeField] private Vector3 versusSetupPosition;
    [SerializeField] private Vector3 versusSetupRotation;
    [SerializeField] private Vector3 settingsPosition;
    [SerializeField] private Vector3 settingsRotation;
    //camera movement speed
    private float speed = 120.0f;
    //camera rotation speed
    private float angleSpeed = 4.0f;
    //position we're moving towards, if any
    private Vector3 targetPos;
    //angle we're rotating towards, if any
    private Quaternion targetAngle;
    //whether we're moving or not
    [SerializeField] private bool isMoving = false;
    void Start()
    {
        transform.position = mainMenuPosition;
        transform.rotation = Quaternion.Euler(mainMenuRotation);
    }

    //if camera is set to be moving, move us closer to our target
    void Update() {
        if (isMoving) {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, (speed * Time.deltaTime));
            transform.rotation = Quaternion.Lerp(transform.rotation, targetAngle, (angleSpeed * Time.deltaTime));
            if ((Vector3.Distance(transform.position, targetPos) < 0.001f) && (transform.rotation == targetAngle))
            {
                isMoving = false;
            }
        }
    }

    public void goToMainMenu() {
        targetPos = mainMenuPosition;
        targetAngle = Quaternion.Euler(mainMenuRotation);
        isMoving = true;
    }
    public void goToVersusSetup() {
        targetPos = versusSetupPosition;
        targetAngle = Quaternion.Euler(versusSetupRotation);
        isMoving = true;
    }
    public void goToSettings() {
        targetPos = settingsPosition;
        targetAngle = Quaternion.Euler(settingsRotation);
        isMoving = true;
    }
}
