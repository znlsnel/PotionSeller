using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ScreenDebug : Singleton<ScreenDebug>
{
        [SerializeField] GameObject _parent;    
        [SerializeField] GameObject _textPrefab;
        int _maxSize = 1; 

        Queue<GameObject> _texts = new Queue<GameObject>();
        GameObject _waitObj = null;

        public void DebugText(string text)
        {
		GameObject go = _waitObj != null ? _waitObj : Instantiate<GameObject>(_textPrefab);


                go.SetActive(true);
                go.GetComponent<TextMeshProUGUI>().text = text;
                go.transform.SetParent(_parent.transform); 

		_texts.Enqueue(go);
                if (_texts.Count > _maxSize)
                {
                        _waitObj = _texts.Peek();
			_texts.Dequeue();

                        _waitObj.transform.SetParent(null);
                        _waitObj.SetActive(false);
		}
	}

 //       int cnt = 0;
 //       float time = 0f;
	//private void Update()
	//{
	//	time += Time.deltaTime;
 //               cnt++;
	//	if (time >= 1.0f)
 //               {
	//		DebugText(cnt.ToString());
 //                       time = 0f;
 //                       cnt = 0;
	//	}
	//}
}
