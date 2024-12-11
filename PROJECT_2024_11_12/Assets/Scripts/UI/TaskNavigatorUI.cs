using System.Collections;
using UnityEngine;

public class TaskNavigatorUI : MonoBehaviour
{
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	[SerializeField] GameObject _player;
	[SerializeField] Vector3 _offset;
	[SerializeField] float _zOffset;

	[Space(10)]
	[SerializeField] GameObject _ingredient;
	[SerializeField] GameObject _potionTable;
	[SerializeField] Counter _counter;
	[SerializeField] CoinSpawner _coinSpawner;
	[SerializeField] GameObject _dungeon;

	[Space(10)]
	[SerializeField] PickupManager _porterPickup;
	[SerializeField] PickupManager _potionTablePickup;

	[Space(10)]
	[SerializeField] RectTransform _rect;
	[SerializeField] RectTransform _characterUIRect;
	[SerializeField] GameObject _characterUI;
	[SerializeField] GameObject _screenUI;


	GameObject _target;
	// 재료가 없다면 -> 던전
	// 재료를 다 모았다면 -> 재료
	// 포션이 나왔다면 -> 포션테이블
	// 카운터에 포션이 없고 포션을 내가 들고 있다면 -> 카운터
	// 돈이 나왔다면 -> 코인생성기


	void Start()
	{
		StartCoroutine(UpdateCurTarget());
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		Vector3 pos = _player.transform.position + _offset;
		transform.position = pos;

		pos = _characterUIRect.localPosition;
		pos.z = _zOffset;
		_characterUIRect.localPosition = pos;

		LookAt();
		UpdateRectUI();
	}

	// 우선순위
	// 5. 던전
	// 4. 재료
	// 2. 포션테이블
	// 1. 카운터
	// 3. 코인
	IEnumerator UpdateCurTarget()
	{
		while (true)
		{
			bool ingredient = _porterPickup.GetItemType() == EItemType.INGREDIENT_POTION &&
			_porterPickup.GetItemCount() > 0;

			// 재료를 들고 있지 않고 테이블에 포션이 없다면
			bool dungeon = !DungeonDoorway.instance.isPlayerInDungeon() && _porterPickup.GetItemCount() == 0 && _potionTablePickup.GetItemCount() == 0;

			// 포션 테이블에 아이템이 나왔다면 
			bool potionTable = _potionTablePickup.GetItemCount() > 0;

			// 포션을 들고 있다면?
			bool counter = _porterPickup.GetItemType() == EItemType.POTION;

			// 코인이 나왔다면 ?
			bool coin = _coinSpawner.GetCoinCnt() > 0;

			if (counter)
				_target = _counter.gameObject;
			else if (potionTable)
				_target = _potionTable;
			else if (coin)
				_target = _coinSpawner.gameObject;
			else if (ingredient)
				_target = _ingredient;
			else if (dungeon)
				_target = _dungeon;
			else
				_target = null;

			_characterUI.SetActive(_target != null && IsRectTransformInView(_rect) == false);
			SetUIActive(_target != null);

			yield return new WaitForSeconds(0.3f);
		}
	}
	public void LookAt(float speed = 720)
	{
		if (_target == null)
			return;

		Vector3 dir = _target.transform.position - transform.position;
		dir.y = 0.0f;
		Quaternion targetRotation = Quaternion.LookRotation(dir);

		// 현재 회전에서 목표 회전으로 부드럽게 회전
		transform.rotation = Quaternion.RotateTowards(
		    transform.rotation,
		    targetRotation,
		    speed * Time.deltaTime
		);
	}
	void SetUIActive(bool active)
	{
		if (_target != null && _screenUI.activeSelf == false && active) 
		{
			UpdateRectUI();
		}
		_screenUI.SetActive(active);

	}
	void UpdateRectUI()
	{
		if (_target == null)
			return;

		Vector2 targetPos = Camera.main.WorldToScreenPoint(_target.transform.position);
		_rect.localPosition = _rect.parent.InverseTransformPoint(targetPos);

	}

	bool IsRectTransformInView(RectTransform rect)
	{

		// 화면 경계를 구함
		Vector2 screenMax = new Vector2(Screen.width, Screen.height)/2;
		Vector2 screenMin =  -screenMax; // (0, 0) 

		// 모든 코너가 화면 경계 내에 있는지 확인
		Debug.Log(_rect.localPosition);

		if (_rect.localPosition.x > screenMin.x && _rect.localPosition.x < screenMax.x &&
			_rect.localPosition.y > screenMin.y && _rect.localPosition.y <  screenMax.y)
		{
			return true; 
		} 
		

		return false; 
	}
}