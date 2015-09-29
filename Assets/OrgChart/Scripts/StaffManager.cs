using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UniRx;



public class StaffManager : SingletonMonoBehaviour<StaffManager> {

  [SerializeField] public RectTransform orgStaffContainer;
  [SerializeField] public NodePresenter rootNode;

  public void Awake ()
  {
    if (this != Instance) {
      Destroy (this);
      return;
    }
    DontDestroyOnLoad (this.gameObject);
  }
	void Start () {
	
	}
}
