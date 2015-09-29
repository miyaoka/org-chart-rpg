using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UniRx;
using System.Collections.Generic;

public class EnemyManager : SingletonMonoBehaviour<EnemyManager> {
  [SerializeField] public RectTransform enemyContainer;
  [SerializeField] public GameObject enemyPrefab;

  private List<Vector2> enemyTable = new List<Vector2> ();

  public void Awake ()
  {
    if (this != Instance) {
      Destroy (this);
      return;
    }
    DontDestroyOnLoad (this.gameObject);
  }
	void Start () {

    createEnemyTable ();

    var count = Random.Range(10,30);
    while (0 < count--) {
      var em = new EnemyModel ();
      var size = Random.Range (1, 8) * 20 + 20;
      em.avatarSizeDelta.Value = new Vector2 (size, size);
      createEnemyNode (em);
    }
	
  }
  void createEnemyTable(){
    var healthLv = 0f;
    var attackLv = 0f;
    while (healthLv < 5f) {
      while (attackLv < 5f) {
        enemyTable.Add(new Vector2(Mathf.Pow(2f, healthLv), Mathf.Pow(2f, attackLv)));
        attackLv += 1f;
      }
      healthLv += 1f;
      attackLv = 0f;
    }    
  }
  /*
   * tableからmin以上totalPoint内の範囲で抽出
   */ 
  public void createEnemies(float minPoint, float totalPoint){

    removeEnemies ();

    while (true) {
      var list = enemyTable.FindAll (v => {
        var pt = v.x * v.y;
        return (minPoint <= pt && pt <= totalPoint);
      });
      if (list.Count == 0) {
        break;
      }

      var enemy = list [Random.Range (0, list.Count)];
      createEnemy (enemy.x, enemy.y);
      totalPoint -= enemy.x * enemy.y;
    }

    foreach (Transform child in enemyContainer) {
      var ep = child.GetComponent<EnemyPresenter> ();
    }
  }
  void createEnemy(float health, float attack){
    var em = new EnemyModel ();
    em.attackStrength.Value = attack;
    em.health.Value = health;
    var size = Mathf.Pow(health, .5f) * 20 + 20;
    em.avatarSizeDelta.Value = new Vector2 (size, size);
    createEnemyNode (em);
  }
	
  void createEnemyNode(EnemyModel em){
    var obj = Instantiate (enemyPrefab);
    obj.transform.SetParent (enemyContainer, false);
    var ep = obj.GetComponent<EnemyPresenter> ();
    ep.enemyModel.Value = em;
  }
  void removeEnemies(){
    foreach(Transform child in enemyContainer){
      Destroy (child.gameObject);
    }
  }
}
