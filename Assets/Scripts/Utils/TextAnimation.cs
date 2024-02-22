using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class TextAnimation : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textMeshPro;
	[SerializeField] GameObject loadingPanel;
	[SerializeField] float timeBtwnChars;

	void Start()
	{
		if(loadingPanel.activeSelf) 
		{
			StartCoroutine(TextVisible());
		}
	}


	private IEnumerator TextVisible()
	{
		textMeshPro.ForceMeshUpdate();

		while (true)
		{
			textMeshPro.text = ".";
			yield return new WaitForSeconds(timeBtwnChars);
			textMeshPro.text = "..";
			yield return new WaitForSeconds(timeBtwnChars);
			textMeshPro.text = "...";
			yield return new WaitForSeconds(timeBtwnChars);
		}
	}
}
