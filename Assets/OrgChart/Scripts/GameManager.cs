using UnityEngine;
using System.Collections;
using UniRx;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
  [SerializeField] public RectTransform orgStaffContainer;
  [SerializeField] public RectTransform recruitsContainer;
  [SerializeField] public Canvas canvas;
  [SerializeField] public GameObject nodePrefab;
  [SerializeField] public GameObject ignore;

  public ReactiveProperty<NodePresenter> draggingNode = new ReactiveProperty<NodePresenter>();


  [SerializeField] GameObject staffNodePrefab;
  [SerializeField] GameObject damagePrefab;
  [SerializeField] GameObject questPrefab;
  [SerializeField] RectTransform questContainer;
  [SerializeField] RectTransform questHealth;

  public ReactiveProperty<int> year = new ReactiveProperty<int> (0);
  public ReactiveProperty<float> money = new ReactiveProperty<float> ();
  public ReactiveProperty<int> manPower = new ReactiveProperty<int> ();
  public ReactiveProperty<int> manCount = new ReactiveProperty<int> ();

  public ReactiveProperty<bool> isDragging = new ReactiveProperty<bool> (false);
  public ReactiveProperty<bool> onQuest = new ReactiveProperty<bool> ();
  public ReactiveProperty<bool> onBattle = new ReactiveProperty<bool> ();
  public ReactiveProperty<int> floor = new ReactiveProperty<int> ();





  public ReactiveProperty<QuestPresenter> selectedQuest = new ReactiveProperty<QuestPresenter> ();
  public ReactiveProperty<string> logText = new ReactiveProperty<string>();

  public IConnectableObservable<long> battleTimer;


  public void Awake ()
  {
    if (this != Instance) {
      Destroy (this);
      return;
    }
    DontDestroyOnLoad (this.gameObject);
  }

  // Use this for initialization
  void Start ()
  {
    Destroy (ignore);
    var root = createNode(orgStaffContainer);
    root.isRoot.Value = true;
    root.tier.Value = 1;

    year.Value++;

    var count = 10;
    while (0 < count--) {
      createRecruit ();
    }
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
    n.staffModel.Value = s;

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

      var s = n.staffModel.Value;
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
