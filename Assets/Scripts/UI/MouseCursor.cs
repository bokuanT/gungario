using UnityEngine;

public class MouseCursor : MonoBehaviour
{
	RectTransform _rect;
	[SerializeField] private CursorLockMode _cursorLockState = CursorLockMode.None;

	void Start()
	{
		if (Input.mousePresent)
		{
			_rect = GetComponent<RectTransform>();

			if (!Application.isEditor)
				Cursor.visible = false;
			Cursor.lockState = _cursorLockState;
			Cursor.visible = false;
		}
		else
			gameObject.SetActive(false);
	}

	private void LateUpdate()
	{
		_rect.position = Input.mousePosition;
	}
}



