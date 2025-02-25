using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;
//using static UnityEditor.PlayerSettings;

public class CoinSpawner : MonoBehaviour
{
	[SerializeField] GameObject _coinPrefab;
	[SerializeField] GameObject _spotLight;
	[SerializeField] Transform _coinPosition;
	ObjectPool<GameObject> _pool;

	[SerializeField] Vector3 _coinOffset;
	Stack<GameObject> _coins = new Stack<GameObject>();
	int _coinCnt = 0;
	GameObject _player;

	[Space(10)]
	[SerializeField] AudioClip _coinPickupAudio;
	public int GetCoinCnt()
	{
		return _coins.Count;
	}
	private void Start()
	{
		_spotLight.SetActive(false);
		_pool = new ObjectPool<GameObject>(
			createFunc: () => Instantiate<GameObject>(_coinPrefab),
			actionOnGet: (obj) => {
				
				SetCoinTransform(obj);
				obj.SetActive(true);
				obj.transform.SetParent(null);
				obj.GetComponent<Item>().AddReleaseAction(() => { _pool.Release(obj); });
			}
			) ;
		StartCoroutine(CheckSpotLight());
	}
	IEnumerator CheckSpotLight()
	{
		while (true)
		{
			_spotLight.SetActive(_coins.Count > 0);
			yield return new WaitForSeconds(0.3f);
		}

	}
	public void AddCoin(int cnt)
	{
		_spotLight.SetActive(true);
		_coinCnt += cnt;
		while (_coinCnt < 300 && cnt-- > 0)
		{
			_pool.Get();
		}

		Vector3 pos = _spotLight.transform.position;
		pos.y = (_coins.Count == 0 ? _coinPosition.position.y : _coins.Peek().transform.position.y) + 5.0f;  
		_spotLight.transform.position = pos;
		 
		if (_player != null && _sendCoin == null)
		{	
			Utils.instance.SetTimer(() => 
			{ 
				if (_player != null)
					_sendCoin = StartCoroutine(SendCoin(_player.GetComponent<PickupManager>()));
			}, 0.3f); 
		}
	}

	void SetCoinTransform(GameObject coin)
	{
		coin.transform.SetParent(null);
		coin.transform.position = _coinPosition.position;
		coin.transform.rotation = _coinPosition.rotation;
		int size_X = 3;
		int size_Z= 3;


		Renderer renderer = _coins.Count == 0 ? null : _coins.Peek().GetComponent<Renderer>();
		if (renderer != null )
		{
			int yIdx = _coins.Count / (size_X * size_Z);
			int zIdx = _coins.Count % (size_X * size_Z) / size_X;
			int xIdx = _coins.Count % (size_X * size_Z) % size_X;

			Vector3 pos = _coinPosition.position;
			pos.y += yIdx * (renderer.bounds.size.y + _coinOffset.y);
			pos.z +=  -zIdx * (renderer.bounds.size.z + _coinOffset.z);
			pos.x += xIdx * (renderer.bounds.size.x + _coinOffset.x);

			//Debug.Log($"Start Pos : [{_coinPosition.position.x}, {_coinPosition.position.y}, {_coinPosition.position.z}]");
		//	Debug.Log($"bounding size : [{renderer.bounds.size.x}, {renderer.bounds.size.y}, {renderer.bounds.size.z}]");
		//	Debug.Log($"coinOffset size : [{_coinOffset.x}, {_coinOffset.y}, {_coinOffset.z}]");
		///	Debug.Log($"XYZ IDX  : [{xIdx}, {yIdx}, {zIdx}]"); 

			coin.transform.position = pos;
		}

		_coins.Push(coin);
	}

	Coroutine _sendCoin;
	private void OnTriggerEnter(Collider other)
	{
		LayerMask findLayerMask = LayerMask.GetMask("Player");
		if ((findLayerMask.value & (1 << other.gameObject.layer)) == 0)
			return;
		_player = other.gameObject;

		if (_sendCoin == null)
			_sendCoin = StartCoroutine(SendCoin(other.GetComponent<PickupManager>()));

	}

	private void OnTriggerExit(Collider other)
	{
		LayerMask findLayerMask = LayerMask.GetMask("Player");
		if ((findLayerMask.value & (1 << other.gameObject.layer)) == 0)
			return;

		if (_sendCoin != null)
		{
			StopCoroutine(_sendCoin);
			_sendCoin = null;
		} 

		_player = null ;
	}

	IEnumerator SendCoin(IItemReceiver receiver)
	{
		float time = 1.0f;
		int sendCnt = Mathf.Max(1, _coins.Count / 30);
		while (_coins.Count > 0) 
		{
			float t = time / (_coins.Count / sendCnt); 
			time -= Time.deltaTime;
			AudioManager.instance.PlayAudioClip(_coinPickupAudio);
			for (int i = 0; i < sendCnt && _coins.Count > 0; i++)
			{ 
				int sendCoin = (_coinCnt / _coins.Count);
				if (_coinCnt > _coins.Count && _coinCnt % _coins.Count > 0)
					sendCoin++;

				_coinCnt -= sendCoin;
				receiver.ReceiveItem(_coins.Peek());
				_coins.Pop();

				 
				CoinUI.instance.AddCoin(Mathf.Min(_coinCnt, sendCoin) * DataBase.instance._potionPrice.GetValue());
			} 
			yield return new WaitForSeconds(t);
		}
		_sendCoin = null;
	}
}
