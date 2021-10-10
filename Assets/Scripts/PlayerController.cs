using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public Vector2 moveValue;
    public float speed;
    private int count;
    private int numPickUps = 4;
    public Vector3 lastPosition;
    public Vector3 currentPosition;
    public Text scoreText;
    public Text winText;
    public Text positionText;
    public Text velocityText;
    public Text speedText;
    private GameObject nearestPickUp;
    public Text distanceToNearestPickup;
    private LineRenderer lineRenderer;
    enum DebugModes {Normal, Distance, Vision};
    DebugModes currentMode;

    void Start() {
        count = 0;
        currentPosition = transform.position;
        winText.text = "";
        currentMode = DebugModes.Normal;
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.0f;
        lineRenderer.endWidth = 0.0f;
        SetCountText();
    }

    void OnMove(InputValue value) {
        moveValue = value.Get<Vector2>();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            switch (currentMode) {
                case DebugModes.Normal:
                    currentMode = DebugModes.Distance;
                    SetPositionVelocitySpeedText();
                    FindNearestPickUp();
                    break;
                case DebugModes.Distance:
                    currentMode = DebugModes.Vision;
                    nearestPickUp.GetComponent<Renderer>().material.color = Color.white;
                    SetPositionVelocitySpeedText();
                    DrawVelocityPointer();
                    break;
                case DebugModes.Vision:
                    currentMode = DebugModes.Normal;
                    positionText.text = "";
                    velocityText.text = "";
                    speedText.text = "";
                    distanceToNearestPickup.text = "";
                    nearestPickUp.GetComponent<Renderer>().material.color = Color.white;
                    lineRenderer.startWidth = 0.0f;
                    lineRenderer.endWidth = 0.0f;
                    break;
            }
        }
    }

    void FixedUpdate() {
        Vector3 movement = new Vector3(moveValue.x, 0.0f, moveValue.y);
        GetComponent<Rigidbody>().AddForce(movement * speed * Time.fixedDeltaTime);
        if (currentMode == DebugModes.Distance)
        {
            SetPositionVelocitySpeedText();
            FindNearestPickUp();
        }
        if (currentMode == DebugModes.Vision) {
            SetPositionVelocitySpeedText();
            DrawVelocityPointer();
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "PickUp") {
            other.gameObject.SetActive(false);
            count++;
            SetCountText();
        }
    }

    private void SetCountText() {
        scoreText.text = "Score: " + count.ToString();
        if (count >= numPickUps) {
            winText.text = "You Win!";
        }
    }

    private void SetPositionVelocitySpeedText() {
        lastPosition = currentPosition;
        currentPosition = transform.position;
        positionText.text = "Position: " + currentPosition.ToString();
        Vector3 velocity = (currentPosition - lastPosition) / Time.deltaTime;
        velocityText.text = "Velocity: " + velocity.ToString();
        speedText.text = "Speed: " + (velocity.magnitude).ToString();
        //velocityText.text = "Velocity: " + ((GetComponent<Rigidbody>()).velocity.magnitude).ToString();
    }

    private void FindNearestPickUp() {
        GameObject[] pickUps = GameObject.FindGameObjectsWithTag("PickUp");
        nearestPickUp = null;
        float currentDistanceToNearestPickUp = Mathf.Infinity;
        foreach (GameObject j in pickUps)
        {
            j.GetComponent<Renderer>().material.color = Color.white;
        }
        foreach (GameObject i in pickUps)
        {
            float calculatedDistance = Vector3.Distance(currentPosition, i.transform.position);
            if ( calculatedDistance < currentDistanceToNearestPickUp)
            {
                currentDistanceToNearestPickUp = calculatedDistance;
                nearestPickUp = i;
            }
        }
        nearestPickUp.GetComponent<Renderer>().material.color = Color.blue;
        distanceToNearestPickup.text = "Distance to nearest pick up: " + (currentDistanceToNearestPickUp).ToString();
        lineRenderer.SetPosition(0, nearestPickUp.transform.position);
        lineRenderer.SetPosition(1, currentPosition);
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
    }

    private void DrawVelocityPointer() {
        Vector3 playerVelocity = (currentPosition - lastPosition) / Time.deltaTime;
        lineRenderer.SetPosition(0, currentPosition);
        lineRenderer.SetPosition(1, playerVelocity + currentPosition);
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        GameObject[] pickUps = GameObject.FindGameObjectsWithTag("PickUp");
        GameObject atDirectApproach = null;
        float currentDistanceToObjectAtDirectApproach = Mathf.Infinity;
        foreach (GameObject j in pickUps)
        {
            j.GetComponent<Renderer>().material.color = Color.white;
        }
        foreach (GameObject i in pickUps)
        {
            float calculatedDistance = Vector3.Distance((playerVelocity + currentPosition), i.transform.position);
            if (calculatedDistance < currentDistanceToObjectAtDirectApproach)
            {
                currentDistanceToObjectAtDirectApproach = calculatedDistance;
                atDirectApproach = i;
            }
        }
        atDirectApproach.GetComponent<Renderer>().material.color = Color.green;
        atDirectApproach.transform.LookAt(this.transform);
    }
}
