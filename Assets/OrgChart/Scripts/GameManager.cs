using UnityEngine;
using System.Collections;
using UniRx;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;
public enum GameState
{
  Title,
  GameEnter,
  Organizing,
  QuestEnter,
  FloorEnter,
  Battle,
  Looting,
  FloorExit,
  QuestExit,
  PeriodClosing,
  GameExit
}

public class GameManager : SingletonMonoBehaviour<GameManager>
{
  [SerializeField] public RectTransform orgStaffContainer;
  [SerializeField] public RectTransform recruitsContainer;
  [SerializeField] public RectTransform enemyContainer;
  [SerializeField] public Canvas canvas;
  [SerializeField] public GameObject nodePrefab;
  [SerializeField] public GameObject enemyPrefab;
  [SerializeField] public GameObject ignore;

  public ReactiveProperty<NodePresenter> draggingNode = new ReactiveProperty<NodePresenter>();


  [SerializeField] GameObject damagePrefab;
  [SerializeField] GameObject modalDialogPrefab;

  public ReactiveProperty<int> year = new ReactiveProperty<int> (0);
  public ReactiveProperty<float> money = new ReactiveProperty<float> ();
  public ReactiveProperty<int> manPower = new ReactiveProperty<int> ();
  public ReactiveProperty<int> manCount = new ReactiveProperty<int> ();
  public ReactiveProperty<int> salary = new ReactiveProperty<int> ();

  public ReactiveProperty<bool> isDragging = new ReactiveProperty<bool> (false);
  public ReactiveProperty<bool> onQuest = new ReactiveProperty<bool> ();
  public ReactiveProperty<int> floor = new ReactiveProperty<int> ();





  public ReactiveProperty<QuestPresenter> selectedQuest = new ReactiveProperty<QuestPresenter> ();
  public ReactiveProperty<string> logText = new ReactiveProperty<string>();


  public ReactiveProperty<GameState> gameState = new ReactiveProperty<GameState> (GameState.GameEnter);


  //戦闘処理のupdateEvent
  public IConnectableObservable<float> battleUpdate;
  public ReactiveProperty<bool> onBattle = new ReactiveProperty<bool> ();

  public void Awake ()
  {
    if (this != Instance) {
      Destroy (this);
      return;
    }
    DontDestroyOnLoad (this.gameObject);


    //    Time.timeScale = 2f;
    Destroy (ignore);
    var root = createNode(orgStaffContainer);
    root.isRoot.Value = true;
    root.tier.Value = 1;


    manCount = 
      root
        .manCountTotal
        .ToReactiveProperty ();

    salary = 
      root
        .salaryTotal
        .ToReactiveProperty ();
  }

  // Use this for initialization
  void Start (){


    var battleUpdateConnect = default(IDisposable);
    battleUpdate = 
      Observable
        .EveryUpdate ()
        .Select(f => Time.deltaTime)
        .Publish ();
    onBattle
      .Subscribe (b => {
        if(b){
          battleUpdateConnect = battleUpdate.Connect();
        } else if(battleUpdateConnect != null) {
          battleUpdateConnect.Dispose();
        }
      }).AddTo (this);

    gameEnter ();

  }

  /*
  public void attackToQuest(NodePresenter s)
  {
    var q = selectedQuest.Value;
    if (q == null) {
      return;
    }

    var dmgObj = Instantiate(damagePrefab);
    dmgObj.transform.SetParent (questHealth.transform, false);
    var dp = dmgObj.GetComponent<DamagePresenter> ();

    var log = s.staff.Value.name.Value + "の攻撃、";
    if (.5f > UnityEngine.Random.value) {
      var d = s.currentLevel.Value;
      dp.pop (d.ToString ());
      GameSounds.hit.Play ();
      log += d.ToString () + "ダメージ与えた！";
      q.health.Value -= (float)d;

      //clear quest
      if (0 >= q.health.Value) {
        onQuest.Value = false;
        selectedQuest.Value = null;
        money.Value += q.reward.Value;
        GameSounds.promote.Play ();
        endYear ();
      }
    } else {
      GameSounds.miss.Play ();
      dp.pop ("miss", Color.white, 20);
      log += "ミス";
    }
    logText.Value += log + "\n";
  }
  */

  /*
  void playerAttack(){
  }
  void enemyAttack(){
    var activeNodes = new List<NodePresenter> ();
    NodePresenter[] nodes = orgStaffContainer.GetComponentsInChildren<NodePresenter> ();
    foreach (NodePresenter n in nodes) {
      if(n.model.Value == null || 0 >= n.model.Value.health.Value) {
        continue;
      }
      activeNodes.Add (n);
    }
    if (1 > activeNodes.Count) {
      return;
    }

    var target = activeNodes [Random.Range (0, activeNodes.Count)];
  
  }
  public void attack(float attack, NodePresenter target)
  {


    var dmgObj = Instantiate(damagePrefab);
    dmgObj.transform.SetParent (target.transform, false);
    var dp = dmgObj.GetComponent<DamagePresenter> ();

    if (.5f > Random.value) {
      target.model.Value.damage.Value += attack;
      dp.pop (attack.ToString ());
      GameSounds.damage.Play ();

      if (0 >= target.model.Value.health.Value) {
//        log += sname + "は死亡した";
      }
    } else {
      dp.pop ("miss", Color.white, 20);
    }

  }
  */


  void gameEnter(){
    Debug.Log ("init");
    gameState.Value = GameState.GameEnter;
    year.Value = 0;
    money.Value = 3000;

    organizing ();
  }
  void organizing(){
    gameState.Value = GameState.Organizing;

    year.Value++;
   
    removeRecruits ();
    var count = 10;
    while (0 < count--) {
      createRecruit ();
    }

    return;
    var f = 0f;

    while (f < 10f) {
      Debug.Log ((f+1).ToString () + "F");
      var pt = Mathf.Pow (1.2f, f) + f * 10f + 10f;

//      createEnemies (pt * .05f, pt * Random.Range(.75f,1f));
      f++;
    }
  }
  void removeRecruits(){
    foreach(Transform child in recruitsContainer){
      Destroy (child.gameObject);
    }
  }


  public void periodExit(){
    Debug.Log ("p exit");

    gameState.Value = GameState.Organizing;
  }
  /*
   * ダンジョン突入
   */ 
  public void questEnter(){
    removeRecruits ();
    gameState.Value = GameState.QuestEnter;
    floor.Value = 0;

    floorEnter ();
  }
  /*
   * 指定階へ
   */ 
  void goFloor(int floorNum){
    floor.Value = floorNum;

  }
  /*
   * 戦闘突入
   */ 
  void goBattle(){
  }
  /*
   * 新階層突入
   */ 
  public void floorEnter(){
    gameState.Value = GameState.FloorEnter;
    floor.Value++;
    onBattle.Value = true;

    var f = floor.Value;
    Debug.Log ((f+1).ToString () + "F");
    var pt = Mathf.Pow (1.2f, f) + f * 5f + 10f;

    EnemyManager.Instance.createEnemies (pt * .05f, pt * Random.Range(.75f,1f));


  }
  /*
   * 次階層へ進むかどうか
   */ 
  void floorExit(){
    Debug.Log ("floor exit");
  }
  public void questExit(){
    Debug.Log ("quest exit");
    onBattle.Value = false;

    organizing ();
  }

  public EnemyPresenter createEnemyNode(EnemyModel em){
    var obj = Instantiate (enemyPrefab);
    obj.transform.SetParent (enemyContainer, false);
    var ep = obj.GetComponent<EnemyPresenter> ();
    ep.enemyModel.Value = em;

    return ep;
  }

  public NodePresenter createNode(Transform parent = null){
    var obj = Instantiate (nodePrefab);
    parent = parent ?? canvas.transform;
    obj.transform.SetParent (parent, false);
    var node = obj.GetComponent<NodePresenter> ();
    return node;
  }
  void createRecruit(){
    var n = createNode (recruitsContainer);
    var s = new StaffModel();
    n.model.Value = s;

    var ss = (float)NormalDistributionConfidenceCalculator.NormInv ((double)Random.value, .5d, .1d);
    s.stdScore.Value = ss;

    var age = Random.Range (0, StaffModel.ageSpan +5);
    s.age.Value = 0;
    s.baseLevel.Value = Random.Range(1, 3);

    while (0 < age--) {
      addAge (s);
    }
    s.baseLevel.Value = (int)Mathf.Max(1, Mathf.Floor((float)s.baseLevel.Value * .85f));
    s.lastLevel.Value = -1;

    s.hiringCost.Value = (int)
      (
        (Mathf.Pow ((float)(s.baseLevel.Value - 1), 2f) * 10f + 100f) 
        * (s.isRetireAge() ? .5f : 1f) 
      );

    s.name.Value = "";
    s.hue.Value = Mathf.Floor (Random.value * 3) / 3 + (.2f > Random.value ? 1f/6f : 0);
    s.moral.Value = Random.value * .6f + .3f;

    s.skinColor.Value = Util.HSVToRGB (.1f, UnityEngine.Random.Range(.25f, .8f), UnityEngine.Random.Range (1f, 1f));
    s.hairColor.Value = Util.HSVToRGB (UnityEngine.Random.Range(0,1f), .5f, UnityEngine.Random.Range (.3f, .65f));
    s.clothColor.Value = Util.HSVToRGB (UnityEngine.Random.Range(0,1f), .4f, .7f);

  }
  void addAge(StaffModel sm){
    sm.grow ();
    sm.grow ();
    sm.grow ();
    sm.grow ();
    sm.age.Value++;
  }
  public void nextYear(){

    NodePresenter[] nodes = orgStaffContainer.GetComponentsInChildren<NodePresenter> ();
    foreach (NodePresenter n in nodes) {

      var s = n.model.Value;
      if (s == null) {
        continue;
      }
      addAge (s);
      if (s.age.Value >= 45 && n.tier.Value > 1) {
        n.isAssigned.Value = false;
      }
    }

    foreach(Transform t in recruitsContainer)
    {
      Destroy (t.gameObject);
    }

    var count = 3;
    while (0 < count--) {
      createRecruit ();
    }
  }

  public void gotoQuest(){
    //change bg
    //hide recruit
    floor.Value = 0;
    gotoNextFloor();
  }
  public void gotoNextFloor(){
    floor.Value += 1;
    //generate enemy
    //show floor roll
    startBattle();
  }
  void startBattle(){
    onBattle.Value = true;
  }
  public void retreatBattle(){
    onBattle.Value = false;
  }
  void winBattle(){
    onBattle.Value = false;
  }
  void loseBattle(){
    onBattle.Value = false;
  }
  void showResult(){
  }
  public void gotoTown(){
    //change bg

  }



}
