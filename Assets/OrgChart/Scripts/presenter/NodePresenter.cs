using UnityEngine;
using System.Collections;
using UniRx;
using UniRx.Triggers;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class NodePresenter : CombatantPresenter {

  //view
  [SerializeField] public RectTransform childNodes;
  [SerializeField] public GameObject panelUI;
  [SerializeField] public GameObject panelContentUI;
  [SerializeField] public GameObject staffPanelUI;
  [SerializeField] public GameObject emptyPanelUI;
  [SerializeField] public GameObject dropAreaUI;
  [SerializeField] Image parentLine;
  [SerializeField] Image childrenLineH;
  [SerializeField] Image childrenLineV;


  //model
  public ReactiveProperty<int> childCount { get; private set; }
  public ReactiveProperty<int> childCountTotal = new ReactiveProperty<int>();
  public ReactiveProperty<int> currentLevel = new ReactiveProperty<int>();
  public ReactiveProperty<int> currentLevelTotal = new ReactiveProperty<int>();
  public ReactiveProperty<int> manCount = new ReactiveProperty<int>();
  public ReactiveProperty<int> manCountTotal = new ReactiveProperty<int>();
  public ReactiveProperty<int> salary = new ReactiveProperty<int>();
  public ReactiveProperty<int> salaryTotal = new ReactiveProperty<int>();

  public ReadOnlyReactiveProperty<bool> hasChild { get; private set; }

  public ReactiveProperty<int> tier = new ReactiveProperty<int> (0);
  public ReactiveProperty<bool> isHired = new ReactiveProperty<bool> ();
  public ReactiveProperty<bool> isRoot = new ReactiveProperty<bool> ();

  public ReactiveProperty<bool> isAssigned = new ReactiveProperty<bool> (false);
  public ReadOnlyReactiveProperty<bool> isEmpty { get; private set; }


  public ReactiveProperty<NodePresenter> parentNode = new ReactiveProperty<NodePresenter>();
  public ReactiveProperty<int?> parentDiff = new ReactiveProperty<int?>();

  public ReactiveProperty<StaffModel> model = new ReactiveProperty<StaffModel> ();

  CompositeDisposable childResources = new CompositeDisposable();
  CompositeDisposable staffResources = new CompositeDisposable();

  private float dropOverScale = 1.6f;
  private float dragAlpha = .6f;

  void Awake(){


    //子ノードの数
    childCount = childNodes
      .ObserveEveryValueChanged(t => t.childCount)
      .ToReactiveProperty ();

    hasChild = childCount
      .Select (c => 0 < c)
      .ToReadOnlyReactiveProperty ();

    //割当人員の数
    manCount =
      isAssigned
        .Select (a => a ? 1 : 0)
        .ToReactiveProperty ();

    //給料
    salary =
      model
        .Select (sm => sm != null ? sm.baseLevel.Value : 0)
        .ToReactiveProperty ();


    //if have parent node
    parentNode
      .Where (pn => pn != null)
      .Subscribe (pn => {
        parentDiff = pn.currentLevel
          .CombineLatest(currentLevel, (l, r) => (int?)l - r)
          .CombineLatest(pn.isEmpty, (l, r) => r ? null : l)
          .ToReactiveProperty ();
      })
      .AddTo(this);

    parentNode
      .Select (p => p != null)
      .Subscribe (p => parentLine.gameObject.SetActive (p))
      .AddTo (this);

    childCount
      .Select (c => 0 < c)
      .Subscribe (c => childrenLineV.gameObject.SetActive (c))
      .AddTo (this);

    childCount
      .Select (c => 1 < c)
      .Subscribe (c => childrenLineH.gameObject.SetActive (c))
      .AddTo (this);

    childCount
      .Subscribe (_ => watchChildProps ())
      .AddTo (this);



    //if have staff
    model
      .Subscribe (s => {

        staffResources.Clear();
        if(s == null){
          isAssigned.Value = false;
          return;
        }
        isAssigned.Value = true;

        s.baseLevel
          .CombineLatest (childCount, (l, r) =>  Mathf.Max(1, l - r))
          .Subscribe(b => currentLevel.Value = b)
          .AddTo(staffResources);


      })
      .AddTo (this);



    /*

        manCount =
            isEmpty
                .Select (a => a ? 0 : 1)
                .ToReactiveProperty ();

        //subscribe
        childCount
            .Subscribe (_ => watchChildSum ())
            .AddTo (this);

        */

    //        watchChildSum ();



  }
  void Start(){
    var gm = GameManager.Instance;
    var drag = staffPanelUI.GetComponent<DragTrigger> ();
    var drop = dropAreaUI.GetComponent<DropTrigger> ();
    var panelOutLine = panelContentUI.GetComponent<Outline> ();

    var panelCG = panelContentUI.GetComponent<CanvasGroup>();
    var staffPanelCG = staffPanelUI.GetComponent<CanvasGroup> ();
    var dropCG = dropAreaUI.GetComponent<CanvasGroup> ();

    var isDragged = gm.draggingNode
      .Select (n => n == this)
      .ToReactiveProperty ();

    //アサインなし or ドラッグ中 -> empty
    isEmpty = isAssigned
      .CombineLatest (isDragged, (l, r) => !l || r)
      .ToReadOnlyReactiveProperty ();


    //show or hide staffpanel
    isEmpty
      .Subscribe (e => {
        staffPanelCG.alpha = e ? 0 : 1;
        staffPanelCG.blocksRaycasts = !e;
        emptyPanelUI.SetActive(e);
      })
      .AddTo (this);

    //enable drop if other node is dragging
    gm.draggingNode
      .Select (n => n && n != this)
      .Subscribe (d => {
        panelCG.blocksRaycasts = !d;
        dropCG.blocksRaycasts = d;
      })
      .AddTo (this);

    //destory if no content
    isDragged
      .CombineLatest (isAssigned, (l, r) => l || r)
      .CombineLatest (hasChild, (l, r) => l || r)
      .CombineLatest (isRoot, (l, r) => l || r)
      .Where(exist => !exist)
      .Subscribe (_ => Destroy (gameObject))
      .AddTo (this);


    //drag trigger
    drag.OnBeginDragAsObservable ()
      .Subscribe (_ => {
        //create cursor for drag
        var cursor = gm.createNode();
        cursor.model.Value = model.Value;
        var cursorCG = cursor.GetComponent<CanvasGroup>();
        cursorCG.blocksRaycasts = false;
        cursorCG.alpha = dragAlpha;
        cursor.tier.Value = tier.Value;

        //set drag node
        gm.draggingNode.Value = this;

        drag.OnDragAsObservable ()
          .Subscribe (d => {
            (cursor.transform as RectTransform).position = Input.mousePosition;
          })
          .AddTo(cursor);

        drag.OnEndDragAsObservable()
          .Subscribe(e => {
            //remove cursor
            Destroy (cursor.gameObject);

            //clear drag node
            gm.draggingNode.Value = null;
          })
          .AddTo(cursor);
      })
      .AddTo (this);

    //drop trigger
    drop.OnDropAsObservable ()
      .Subscribe (e => {
        var dragNode = e.pointerDrag.GetComponentInParent<NodePresenter>();

        //新規雇用なら雇用費を支払い
        if(!dragNode.isHired.Value){
          GameSounds.accounting.Play();
          gm.money.Value -= dragNode.model.Value.hiringCost.Value;
        } 
        else{
          GameSounds.drop.Play();
        }
        //create child or copy value
        if(isAssigned.Value)
        {
          var child = gm.createNode(childNodes);
          child.model.Value = dragNode.model.Value;
          child.parentNode.Value = this;
          child.tier.Value = tier.Value + 1;
          child.isHired.Value = true;
        }else{
          model.Value = dragNode.model.Value;
          isHired.Value = true;
        }

        //clear pointer value
        dragNode.model.Value = null;
        gm.draggingNode.Value = null;

        //highlight
        panelContentUI.transform.localScale = Vector3.one;
        panelOutLine.effectColor = Color.black;
      })
      .AddTo (this);

    drop.OnPointerEnterAsObservable ()
      .Subscribe (_ => {
        panelContentUI.transform.localScale = new Vector3(dropOverScale, dropOverScale, 1f);
        panelOutLine.effectColor = Color.red;
      })
      .AddTo (this);

    drop.OnPointerExitAsObservable ()
      .Subscribe (_ => {
        panelContentUI.transform.localScale = Vector3.one;
        panelOutLine.effectColor = Color.black;
      })
      .AddTo (this);    

  }
  /*
   * 子の数の変更時に子内容の監視を作り直す
   */
  void watchChildProps(){
    childResources.Clear ();

    var lvList = new List<ReactiveProperty<int>> {currentLevel};
    var ccList = new List<ReactiveProperty<int>> {childCount};
    var mcList = new List<ReactiveProperty<int>> {manCount};
    var saList = new List<ReactiveProperty<int>> {salary};

    foreach (Transform child in childNodes) {
      var node = child.GetComponent<NodePresenter> ();
      lvList.Add (node.currentLevelTotal);
      ccList.Add (node.childCountTotal);
      mcList.Add (node.manCountTotal);
      saList.Add (node.salaryTotal);
    }

    Observable
      .CombineLatest (lvList.ToArray ())
      .Select (list => list.Sum())
      .Subscribe (v => currentLevelTotal.Value = v)
      .AddTo (childResources);

    Observable
      .CombineLatest (ccList.ToArray ())
      .Select (list => list.Sum())
      .Subscribe (v => childCountTotal.Value = v)
      .AddTo (childResources);

    Observable
      .CombineLatest (mcList.ToArray ())
      .Select (list => list.Sum ())
      .Subscribe (v => manCountTotal.Value = v)
      .AddTo (childResources);    

    Observable
      .CombineLatest (saList.ToArray ())
      .Select (list => list.Sum ())
      .Subscribe (v => salaryTotal.Value = v)
      .AddTo (childResources);    

    drawFamilyLine ();

  }
  void drawFamilyLine()
  {
    if (1 < childCount.Value) {
      var firstC = childNodes.GetChild (0);
      var lastC = childNodes.GetChild (childCount.Value - 1);

      var w = childNodes
        .ObserveEveryValueChanged (t => t.sizeDelta)
        .Select (d => d.x / 2)
        .ToReactiveProperty();
      var left = (firstC.transform as RectTransform)
        .ObserveEveryValueChanged (t => t.sizeDelta)
        .Select (d => d.x / 2)
        .ToReactiveProperty ();
      var right = (lastC.transform as RectTransform)
        .ObserveEveryValueChanged (t => t.sizeDelta)
        .Select (d => d.x / 2)
        .ToReactiveProperty ();

      left
        .CombineLatest(right, (l, r) => new Vector2(l, r))
        .CombineLatest(w, (l, r) => new Vector2(-r + l.x, r - l.y))
        .Subscribe (v => {
          childrenLineH.rectTransform.offsetMin = new Vector2(v.x, childrenLineH.rectTransform.offsetMin.y);
          childrenLineH.rectTransform.offsetMax = new Vector2(v.y, childrenLineH.rectTransform.offsetMax.y);
        })
        .AddTo(childResources);
    }    
  }

  void OnDestroy()
  {
    childResources.Dispose ();
    staffResources.Dispose ();
  }

}
