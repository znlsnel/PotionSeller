using TMPro;
using UnityEngine;

public class CustomerChatUI : MonoBehaviour
{
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        [SerializeField] TextMeshProUGUI _potionCntText;
        [SerializeField] RectTransform _rect;
        Transform _uiPos;

	private void Awake()
	{
		_rect.gameObject.SetActive(false);
	}
	public void SetPotionCnt(int cnt)
        {
                _potionCntText.text = cnt.ToString();
        }

        public void SetTransform(Transform ts)
        {
                _uiPos = ts;
	}

	private void FixedUpdate()
	{
		MoveUI();
	}

        void MoveUI()
        {
		if (_uiPos == null)
			return;

		Vector2 pos = Camera.main.WorldToScreenPoint(_uiPos.position);
		_rect.localPosition = _rect.parent.InverseTransformPoint(pos);

	}

        public void SetActive(bool active)
        {
		if (active)
			MoveUI();
		
		_rect.gameObject.SetActive(active);
	}
}

