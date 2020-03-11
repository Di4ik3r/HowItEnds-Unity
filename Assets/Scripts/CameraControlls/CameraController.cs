using UnityEngine;

namespace Assets.Scripts.CameraControlls
{
    public class CameraController : MonoBehaviour
    {
        private static float movementSpeed = 1.0f;        
        private bool cursorVisible = true;

        public static Transform creature;
        public Vector3 cameraOffset;
        public static bool ShouldFollowCreature = false;
        
        [Range(0.01f, 1f)]
        public float smoothFactor;

        void LateUpdate()
        {
            if (ShouldFollowCreature)
            {
                Vector3 newPosition = creature.position + cameraOffset;
                transform.position = Vector3.Lerp(transform.position, newPosition, smoothFactor);                
                transform.LookAt(creature.position);
            }
        }

        void Update()
        {
            if (!PauseMenu.IsGamePaused)
            {
                if(Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.W) || 
                    Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D))
                {
                    ShouldFollowCreature = false;                    
                }

                if(!ShouldFollowCreature)
                {
                    movementSpeed = Mathf.Max(movementSpeed += Input.GetAxis("Mouse ScrollWheel"), 0.0f);
                    transform.position += (transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical") + transform.up * Input.GetAxis("Jump")) * movementSpeed;
                    transform.eulerAngles += new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0);
                }

                if (Input.GetMouseButtonDown(1))
                {
                    if (cursorVisible)
                    {
                        Cursor.visible = false;
                        cursorVisible = false;
                    }
                    else
                    {
                        Cursor.visible = true;
                        cursorVisible = true;
                    }
                }
            }
            else
            {
                Cursor.visible = true;
                cursorVisible = true;
            }
        }
    }
}
