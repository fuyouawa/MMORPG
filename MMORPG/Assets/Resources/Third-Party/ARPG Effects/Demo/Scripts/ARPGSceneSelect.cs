using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace ARPGFX
{

public class ARPGSceneSelect : MonoBehaviour
{
	public bool GUIHide = false;
	public bool GUIHide2 = false;
	
	public void LoadSceneDemo1() {SceneManager.LoadScene("ARPGDemo01");}
	public void LoadSceneDemo2() {SceneManager.LoadScene("ARPGDemo02");}
	public void LoadSceneDemo3() {SceneManager.LoadScene("ARPGDemo03");}
	public void LoadSceneDemo4() {SceneManager.LoadScene("ARPGDemo04");}
	public void LoadSceneDemo5() {SceneManager.LoadScene("ARPGDemo05");}
	public void LoadSceneDemo6() {SceneManager.LoadScene("ARPGDemo06");}
	public void LoadSceneDemo7() {SceneManager.LoadScene("ARPGDemo07");}
	public void LoadSceneDemo8() {SceneManager.LoadScene("ARPGDemo08");}
	public void LoadSceneDemo9() {SceneManager.LoadScene("ARPGDemo09");}
	public void LoadSceneDemo10() {SceneManager.LoadScene("ARPGDemo10");}
	public void LoadSceneDemo11() {SceneManager.LoadScene("ARPGDemo11");}
	public void LoadSceneDemo12() {SceneManager.LoadScene("ARPGDemo12");}
	public void LoadSceneDemo13() {SceneManager.LoadScene("ARPGDemo13");}
	public void LoadSceneDemo14() {SceneManager.LoadScene("ARPGDemo14");}

	void Update ()
	 {
 
     if(Input.GetKeyDown(KeyCode.J))
	 {
         GUIHide = !GUIHide;
     
         if (GUIHide)
		 {
             GameObject.Find("Canvas2").GetComponent<Canvas> ().enabled = false;
         }
		 else
		 {
             GameObject.Find("Canvas2").GetComponent<Canvas> ().enabled = true;
         }
     }
	      if(Input.GetKeyDown(KeyCode.K))
	 {
         GUIHide2 = !GUIHide2;
     
         if (GUIHide2)
		 {
             GameObject.Find("Canvas").GetComponent<Canvas> ().enabled = false;
         }
		 else
		 {
             GameObject.Find("Canvas").GetComponent<Canvas> ().enabled = true;
         }
     }
	 }
}
}