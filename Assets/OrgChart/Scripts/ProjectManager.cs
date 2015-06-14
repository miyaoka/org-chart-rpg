using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UniRx;

public class ProjectManager : MonoBehaviour {
  [SerializeField] GameObject projectPrefab;
  [SerializeField] RectTransform workingProjectContainer;
  [SerializeField] RectTransform planningProjectContainer;

  public List<ProjectPresenter> workingProject { 
    get {
      var list = new List<ProjectPresenter>();
      workingProjectContainer.GetComponentsInChildren<ProjectPresenter> (list);
      return list;
    }
  }

  private static ProjectManager s_Instance;
  public static ProjectManager Instance {
    get {
      if (s_Instance == null) {
        s_Instance = GameObject.FindObjectOfType (typeof(ProjectManager)) as ProjectManager;
      }
      return s_Instance;
    }
  }

  public void createProject(float mp){
    GameObject obj = Instantiate(projectPrefab) as GameObject;
    ProjectPresenter proj = obj.GetComponent<ProjectPresenter> ();

    int id = obj.GetInstanceID();
    proj.title.Value = "proj" + id.ToString ();


    float healthFactor = 5f;
    float healthLevel = UnityEngine.Random.value;
    float attackLevel = UnityEngine.Random.value;
    float minHealth = 5f;
    int health = (int)Mathf.Max (minHealth, Mathf.Ceil (Mathf.Pow (healthFactor, healthLevel - .5f) * mp));
    proj.maxHealth.Value = health;
    proj.health.Value = health;

    proj.attack.Value = (int)Mathf.Floor( attackLevel * mp );

    proj.reward.Value = (int)Mathf.Floor(mp * (1f + healthLevel)  * ( 1f + UnityEngine.Random.value * 2f));

    proj.isSelected
      .Subscribe (v => proj.transform.SetParent (v ? workingProjectContainer : planningProjectContainer))
      .AddTo (proj);


    obj.transform.SetParent (planningProjectContainer);
  }

  public void removePlanning(){
    foreach( Transform t in planningProjectContainer){
      Destroy (t.gameObject);
    }
  }

}
