using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ChangeColorSmooth : MonoBehaviour
{
    public Color color;
	public float duration = 2;
	public Ease colorChangeEase;

	private Color startColor;
	private Material mat;

	private void Awake()
	{
		mat = GetComponent<MeshRenderer>().material;
		startColor = mat.color;
	}

	public void ChangeColorSmoothFunc()
	{
		mat.color = color;
		mat.DOColor(startColor, duration).SetEase(colorChangeEase);
	}

	private void OnApplicationQuit()
	{
		mat.color = startColor;
	}
}
