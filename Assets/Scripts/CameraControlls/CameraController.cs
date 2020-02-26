using UnityEngine;

namespace Assets.Scripts.CameraControlls
{
    public class CameraController : MonoBehaviour
    {
        private static float movementSpeed = 1.0f;

        void Update()
        {
            if (!PauseMenu.IsGamePaused)
            {
                movementSpeed = Mathf.Max(movementSpeed += Input.GetAxis("Mouse ScrollWheel"), 0.0f);
                transform.position += (transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical") + transform.up * Input.GetAxis("Jump")) * movementSpeed;
                transform.eulerAngles += new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0);
            }
        }
    }
}
