using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] Transform playerRoot, lookAroundRoot;
    [SerializeField] bool invert = false;
    [SerializeField] float sensitivity = 5f;
    [SerializeField] float rollAngle = 10f;
    [SerializeField] float rollSpeed = 3f;
    [SerializeField] Vector2 lookLimits = new Vector2(-90f, 90f);
    Vector2 lookAngles;
    Vector2 currentMouseLook;
    Vector2 smoothMove;
    float currentRollAngle;
    int lastLookFrame;

    Vector3 move_direction;
    public float moveSpeed = 5f;
    CharacterController characterController;

    public Transform bulletSpawnPoint;
    public GameObject bulletPrefab;

    bool action = false;

    private void Awake()
    {

    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        cursorLockToggle();

        if (Cursor.lockState == CursorLockMode.Locked)
        {
            LookAround();
        }

        MovePlayer();
        Shoot();

        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        float time = (x != 0 || y != 0) ? 1f : 0.03f;
        float lerpTime = ((x != 0 || y != 0) ? .05f : .5f);

        time = action ? 1 : time;
        lerpTime = action ? 0.1f : lerpTime;

        Time.timeScale = Mathf.Lerp(Time.timeScale, time, lerpTime);
    }

    IEnumerator ActionE(float time)
    {
        action = true;
        yield return new WaitForSecondsRealtime(0.03f);
        action = false;
    }

    private void Shoot()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        }
    }

    private void MovePlayer()
    {
        move_direction = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        move_direction = transform.TransformDirection(move_direction);
        move_direction *= moveSpeed * Time.deltaTime;

        characterController.Move(move_direction);
    }

    void cursorLockToggle()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }

    void LookAround()
    {
        currentMouseLook = new Vector2(Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"));

        lookAngles.x += currentMouseLook.x * sensitivity * (invert ? 1f : -1f);
        lookAngles.y += currentMouseLook.y * sensitivity;
        lookAngles.x = Mathf.Clamp(lookAngles.x, lookLimits.x, lookLimits.y);

        currentRollAngle = Mathf.Lerp(currentRollAngle, Input.GetAxisRaw("Mouse X") * rollAngle, rollSpeed * Time.deltaTime);

        lookAroundRoot.localRotation = Quaternion.Euler(lookAngles.x, 0f, currentRollAngle);
        playerRoot.localRotation = Quaternion.Euler(0f, lookAngles.y, 0f);
    }
}
