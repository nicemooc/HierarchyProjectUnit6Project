using System;
using UnityEngine;

public interface IContextMenu
{
	void Show(Vector2 position);
	void AddItem(string title, Action onClick);
	void AddItem(string title, bool isEnable, Action onClick);
	void AddItem(string title, string subTitle, bool isEnable, Action onClick);
	void AddSeparate();
	void Clear();
	void Close();
}