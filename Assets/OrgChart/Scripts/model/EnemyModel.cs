using UnityEngine;
using System.Collections;
using UniRx;

public class EnemyModel : CombatantModel {
  public ReactiveProperty<Vector2> avatarSizeDelta =  new ReactiveProperty<Vector2> (); 
}
