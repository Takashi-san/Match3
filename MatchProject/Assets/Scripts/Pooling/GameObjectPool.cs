using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectPool : MonoBehaviour {
	GameObject _prefab = null;
	public GameObject Prefab {
		get { return _prefab; }
		set {
			if (_prefab == null) {
				_prefab = value;
			}
			else {
				Debug.LogWarning("Trying to change setted pool prefab.");
			}
		}
	}

	Queue<GameObject> _objects = new Queue<GameObject>();

	public GameObject Get() {
		if (_objects.Count == 0)
			AddObject();
		return _objects.Dequeue();
	}

	public void ReturnToPool(GameObject poolObject) {
		poolObject.SetActive(false);
		_objects.Enqueue(poolObject);
	}

	void AddObject() {
		GameObject newObject = Instantiate(_prefab);
		newObject.SetActive(false);
		newObject.GetComponent<IPooledGameObject>().Pool = this;
		_objects.Enqueue(newObject);
	}
}
