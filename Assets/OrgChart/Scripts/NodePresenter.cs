using UnityEngine;
using System.Collections;
using UniRx;
using UniRx.Triggers;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class NodePresenter : MonoBehaviour {

  //view
  [SerializeField] public RectTransform childNodes;

  //model
  public ReactiveProperty<int> childCount { get; private set; }
  public ReactiveProperty<int> childCountTotal = new ReactiveProperty<int>();
  public ReactiveProperty<int> currentLevel = new ReactiveProperty<int>();
  public ReactiveProperty<int> currentLevelTotal = new ReactiveProperty<int>();
  public ReactiveProperty<int> manCount = new ReactiveProperty<int>();
  public ReactiveProperty<int> manCountTotal = new ReactiveProperty<int>();

  public ReadOnlyReactiveProperty<bool> hasChild { get; private set; }

  public ReactiveProperty<int> tier = new ReactiveProperty<int> (0);
  public ReactiveProperty<bool> isHired = new ReactiveProperty<bool> ();
  public ReactiveProperty<bool> isRoot = new ReactiveProperty<bool> ();

  public ReactiveProperty<bool> isDragging = new ReactiveProperty<bool> ();
  public ReactiveProperty<bool> isAssigned = new ReactiveProperty<bool> (true);
  public ReadOnlyReactiveProperty<bool> isEmpty { get; private set; }


  public ReactiveProperty<StaffNodePresenter> parentNode = new ReactiveProperty<StaffNodePresenter>();
  public ReactiveProperty<int?> parentDiff = new ReactiveProperty<int?>();

  public ReactiveProperty<StaffModel> staff = new ReactiveProperty<StaffModel> ();

  CompositeDisposable childResources = new CompositeDisposable();
  CompositeDisposable staffResources = new CompositeDisposable();


  void Awake(){

    //difine props
    childCount = 
      childNodes
        .ObserveEveryValueChanged(t => t.childCount)
        .ToReactiveProperty ();

    isEmpty =
      isAssigned
        .CombineLatest (isDragging, (l, r) => !l || r)
        .ToReadOnlyReactiveProperty ();
               
    hasChild = 
      childCount
        .Select (c => 0 < c)
        .ToReadOnlyReactiveProperty ();

    manCount =
      isEmpty
        .Select (a => a ? 0 : 1)
        .ToReactiveProperty ();

    //subscribe
    childCount
      .Subscribe (_ => watchChildSum ())
      .AddTo (this);
    
    //destory if no content on no dragging
    isRoot
      .CombineLatest (isDragging, (l, r) => l || r)
      .CombineLatest (isAssigned, (l, r) => l || r)
      .CombineLatest (hasChild, (l, r) => l || r)
      .Where(exist => !exist)
      .Subscribe (_ => Destroy(gameObject))
      .AddTo (this);


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





    //if have staff
    staff
      .Where (s => s != null)
      .Subscribe (s => {

        staffResources.Clear();

        s.baseLevel
          .CombineLatest (childCount, (l, r) =>  l - r)
          .CombineLatest(isEmpty, (l, r) => r ? 0 : l)
          .Subscribe(b => currentLevel.Value = b)
          .AddTo(staffResources);
        /*
        currentLevel
          .Subscribe(c => s.health.Value = c)
          .AddTo(staffResources);
          */

      
      })
      .AddTo (this);

    watchChildSum ();

  }
  void watchChildSum(){
    childResources.Clear ();

    var lvList = new List<ReactiveProperty<int>> {currentLevel};
    var ccList = new List<ReactiveProperty<int>> {childCount};
    var mcList = new List<ReactiveProperty<int>> {manCount};
    foreach (Transform child in childNodes) {
      var node = child.GetComponent<NodePresenter> ();
      lvList.Add (node.currentLevelTotal);
      ccList.Add (node.childCountTotal);
      mcList.Add (node.manCountTotal);
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
  }

  void OnDestroy()
  {
    childResources.Dispose ();
    staffResources.Dispose ();
  }

}
