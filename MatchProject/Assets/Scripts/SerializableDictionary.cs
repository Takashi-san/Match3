using System.Collections.Generic;
using UnityEngine;
using System;

public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver {
	[SerializeField] List<TKey> _keys = new List<TKey>();

	[SerializeField] List<TValue> _values = new List<TValue>();

	public void OnBeforeSerialize() {
		_keys.Clear();
		_values.Clear();

		foreach (var kvp in this) {
			_keys.Add(kvp.Key);
			_values.Add(kvp.Value);
		}
	}

	public void OnAfterDeserialize() {
		this.Clear();

		for (int i = 0; i != Math.Min(_keys.Count, _values.Count); i++)
			this.Add(_keys[i], _values[i]);
	}
}
