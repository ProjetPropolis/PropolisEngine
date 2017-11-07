using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Camera))]
public class mouseDrag : MonoBehaviour
{
	public KeyCode dragKey = KeyCode.Mouse0;
	private Vector3 groundOrigin = Vector3.zero;
	private Vector3 groundNormal = Vector3.up;
	private Plane _groundPlane;
	private Vector3 _dragOrigin;
	private Camera _camera;
	private Transform _transform;

	public int cameraCurrentZoom = 8;
	public int cameraZoomMax = 20;
	public int cameraZoomMin = 5;

	public void Start()
	{
		_camera = GetComponent<Camera>();
		_transform = GetComponent<Transform>();
		_groundPlane = new Plane(groundNormal, groundOrigin);
		Camera.main.orthographicSize = cameraCurrentZoom;
	}
		

	public void Update()
	{
		float distanceToIntersection;
		Ray mouseRay = _camera.ScreenPointToRay(Input.mousePosition);

		if (Input.GetKey(dragKey))
		{
			if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject () == false) {
				if (Input.GetKeyDown(dragKey))
				{
					_groundPlane.Raycast(mouseRay, out distanceToIntersection);
					_dragOrigin = mouseRay.GetPoint(distanceToIntersection);
				}

				if (Input.GetKey(dragKey))
				{
					_groundPlane.Raycast(mouseRay, out distanceToIntersection);
					Vector3 intersection = mouseRay.GetPoint(distanceToIntersection);
					_transform.position += _dragOrigin - intersection;
				}
			}
		}


		//ZOOM SCROLL

		if(Input.GetAxis("Mouse ScrollWheel") < 0 || Input.GetAxis("Mouse ScrollWheel") > 0) {
			if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject () == false) {
				if (Input.GetAxis("Mouse ScrollWheel") < 0) // back
				{
					if (cameraCurrentZoom < cameraZoomMax)
					{
						cameraCurrentZoom += 1;
						Camera.main.orthographicSize = Mathf.Max(Camera.main.orthographicSize + 1);
					} 
				}
				if (Input.GetAxis("Mouse ScrollWheel") > 0) // forward
				{
					if (cameraCurrentZoom > cameraZoomMin)
					{
						cameraCurrentZoom -= 1;
						Camera.main.orthographicSize = Mathf.Min(Camera.main.orthographicSize - 1);
					}   
				}
			}
							
		}
			

	}
}