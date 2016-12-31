# RxCoroutineStream

Overview

UniRxを隠蔽してFromCoroutineを呼ぶ方法はないかな

## Requirement

UniRx required

## Install

Put "RxCoroutineStream.cs" into "Assets" folder

## Code sample

```
public class Main : MonoBehaviour
{
    RxCoroutineStream rx;
    
    void Start(){
        rx = new RxCoroutineStream();
        rx.Publish(StartCoroutine(sequence()))
        .Subscribe(
            stat => {
                Debug.Log("Status#" + stat);
            },
            () => {
                Debug.Log("Complete");
            },
            err => { }
            ).AddTo(this.gameObject);
    }
        
    IEnumerator sequence()
    {
        Debug.Log("Phase 1");
        yield return rx.WaitState("PRESS");
        Debug.Log("Phase 2");
        yield return rx.WaitState("PRESS");
        rx.Complete();
    }
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            rx.ChangeState("PRESS");
        }
    }
}
```

## 目的

UniRxが分からない人にFromCoroutineを使ってもらうために作った
