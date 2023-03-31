using UnityEngine;

namespace UsefulCodes.Radar.Scripts
{
    public class Controller : MonoBehaviour
    {
        public float moveSpeed = 6;
    
    
        private Rigidbody _rigidbody;
        private Camera viewCamera;
        private Vector3 velocity;
    

        // Start is called before the first frame update
        void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            viewCamera=Camera.main;
        }

        // Update is called once per frame
        void Update()
        {
            Vector3 mousePos =
                viewCamera.ScreenToViewportPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,
                    Input.mousePosition.z));
            transform.LookAt(mousePos+Vector3.up*transform.position.y);
            velocity = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized * moveSpeed;
        }

        private void FixedUpdate()
        {
            _rigidbody.MovePosition(_rigidbody.position + velocity * Time.fixedDeltaTime);
        }
    }
}
