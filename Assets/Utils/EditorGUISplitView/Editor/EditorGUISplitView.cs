using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EditorGUISplitView
{
	public enum Direction
	{
		Horizontal,
		Vertical
	}

	Direction _direction;
	float _splitPos;
	bool _resizing;
	public Vector2 _scroll;
	Rect _rect;

	public EditorGUISplitView(Direction splitDirection)
	{
		_splitPos = 0.5f;
		this._direction = splitDirection;
	}

	public void BeginSplitView()
	{
		Rect tempRect;

		if (_direction == Direction.Horizontal)
		{
			tempRect = EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
		}
		else
		{
			tempRect = EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(true));
		}

		if (tempRect.width > 0.0f)
		{
			_rect = tempRect;
		}

		if (_direction == Direction.Horizontal)
		{
			_scroll = GUILayout.BeginScrollView(_scroll, GUILayout.Width(_rect.width * _splitPos));
		}
		else
		{
			_scroll = GUILayout.BeginScrollView(_scroll, GUILayout.Height(_rect.height * _splitPos));
		}
	}

	public void Split()
	{
		GUILayout.EndScrollView();
		ResizeSplitFirstView();
	}

	public void EndSplitView()
	{
		if (_direction == Direction.Horizontal)
		{
			EditorGUILayout.EndHorizontal();
		}
		else
		{
			EditorGUILayout.EndVertical();
		}
	}

	void ResizeSplitFirstView()
	{
		Rect _resizingHandleRect;

		if (_direction == Direction.Horizontal)
		{
			_resizingHandleRect = new Rect(_rect.width * _splitPos, _rect.y, 2f, _rect.height);
		}
		else
		{
			_resizingHandleRect = new Rect(_rect.x, _rect.height * _splitPos, _rect.width, 2f);
		}

		GUI.DrawTexture(_resizingHandleRect, EditorGUIUtility.whiteTexture);

		EditorGUIUtility.AddCursorRect(_resizingHandleRect, _direction == Direction.Horizontal ? MouseCursor.ResizeHorizontal : MouseCursor.ResizeVertical);

		var e = Event.current;
		var eventType = e.type;
		var mousePosition = e.mousePosition;

		if (eventType == EventType.MouseDown && _resizingHandleRect.Contains(mousePosition))
		{
			_resizing = true;
		}
		if (eventType == EventType.MouseUp)
		{
			_resizing = false;
		}

		if (_resizing)
		{
			if (_direction == Direction.Horizontal)
			{
				_splitPos = mousePosition.x / _rect.width;
			}
			else
			{
				_splitPos = mousePosition.y / _rect.height;
			}

			EditorWindow.focusedWindow.Repaint();
		}
	}
}
