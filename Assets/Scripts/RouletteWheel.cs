using UnityEngine;
using System.Collections;

public class RouletteWheel : MonoBehaviour
{
    public Transform wheel; // The wheel to rotate
    public Transform ball;  // The ball to rotate
    public float wheelRotationSpeed = 100f; // Rotation speed of the wheel
    public float ballRotationSpeed = 150f;  // Rotation speed of the ball
    public float stopAfterSeconds = 5f;     // Time to stop rotation
    public int targetPosition = 0;          // Target position (0-36)

    private bool isRotating = true;
    private float elapsedTime = 0f;

    void Update()
    {
        if (isRotating)
        {
            // Rotate the wheel clockwise
            wheel.Rotate(Vector3.back * wheelRotationSpeed * Time.deltaTime);

            // Rotate the ball counter-clockwise
            //ball.Rotate(Vector3.forward * ballRotationSpeed * Time.deltaTime);

            // Increment elapsed time
            elapsedTime += Time.deltaTime;

            // Stop rotation after the specified time
            if (elapsedTime >= stopAfterSeconds)
            {
                isRotating = false;
                StartCoroutine(PlaceBall());
            }
        }
    }

    IEnumerator PlaceBall()
    {
        yield return new WaitForSeconds(1f); // A short delay before placing the ball

        // Calculate the angle for the target position
        float angle = 360f - (360f / 37f * targetPosition); // 37 slots including 0
        // Set the ball's rotation to the target position
        ball.localEulerAngles = new Vector3(0, 0, angle);

        Debug.Log("Ball placed at position: " + targetPosition);
    }
}
