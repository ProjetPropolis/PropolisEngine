using UnityEngine;

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

	public void Start()
	{
		_camera = GetComponent<Camera>();
		_transform = GetComponent<Transform>();
		_groundPlane = new Plane(groundNormal, groundOrigin);
	}

	public void Update()
	{
		float distanceToIntersection;
		Ray mouseRay = _camera.ScreenPointToRay(Input.mousePosition);

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