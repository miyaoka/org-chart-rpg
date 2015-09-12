using UnityEngine;
using System.Collections;

public class StaffController {

  public StaffModel createRecruit(){
    var s = new StaffModel();

    var ss = (float)NormalDistributionConfidenceCalculator.NormInv ((double)Random.value, .5d, .1d);
    s.stdScore.Value = ss;

    var age = Random.Range (0, StaffModel.ageSpan +5);
    s.age.Value = 0;
    s.baseLevel.Value = Random.Range(1, 3);

    while (0 < age--) {
//      addAge (s);
    }
    s.baseLevel.Value = (int)Mathf.Max(1, Mathf.Floor((float)s.baseLevel.Value * .85f));
    s.lastLevel.Value = -1;

    s.name.Value = "";
    s.hue.Value = Mathf.Floor (Random.value * 3) / 3 + (.2f > Random.value ? 1f/6f : 0);
    s.moral.Value = Random.value * .6f + .3f;

    s.skinColor.Value = Util.HSVToRGB (.1f, UnityEngine.Random.Range(.25f, .8f), UnityEngine.Random.Range (1f, 1f));
    s.hairColor.Value = Util.HSVToRGB (UnityEngine.Random.Range(0,1f), .5f, UnityEngine.Random.Range (.3f, .65f));
    s.clothColor.Value = Util.HSVToRGB (UnityEngine.Random.Range(0,1f), .4f, .7f);

    return s;
  }
}
