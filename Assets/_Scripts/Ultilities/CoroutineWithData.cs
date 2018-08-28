/*Alex Greff
19/01/2016
CoroutineWithData
A class that allows coroutines to run and return values
*/
using System.Collections;
using UnityEngine;

//Source: http://answers.unity3d.com/questions/24640/how-do-i-return-a-value-from-a-coroutine.html

public class CoroutineWithData {
     public Coroutine coroutine { get; private set; }
     public object result;
     private IEnumerator target;

     public CoroutineWithData(MonoBehaviour owner, IEnumerator target) {
         this.target = target;
         this.coroutine = owner.StartCoroutine(Run());
     }
 
     private IEnumerator Run() {
         while(target.MoveNext()) {
             result = target.Current;
             yield return result;
         }
     }
 }
