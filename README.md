# RxCoroutineStream

Overview

UniRxを隠蔽してFromCoroutineを呼ぶ方法はないかな

## Requirement

Unity 5.x / UniRx  required

## Install

Put "RxCoroutineStream.cs" into "Assets" folder

## Sample code

```
public class Main : MonoBehaviour
{
    RxCoroutineStream rx;
    
    void Start(){
        rx = new RxCoroutineStream();
        rx.Publish(StartCoroutine(sequence()))
        .Subscribe(
            stat => {
                //ステータスが変更された
            },
            () => {
                //すべて完了した
                Debug.Log("Complete");
            },
            err => { 
                //途中でエラーが発生した
                Debug.Log("Abort " + err.Message);
            }
            ).AddTo(this.gameObject);
    }
        
    IEnumerator sequence()
    {
        yield return rx.WaitState("st-1");
        Debug.Log("Phase 1 が実行された");  
        yield return rx.WaitState("st-2");
        Debug.Log("Phase 2 が実行された");        
        rx.Complete();
    }

    void Update()
    {
        //ステータスを変更する処理
        if (Input.GetKey(KeyCode.A)) {
            rx.ChangeState("st-1");
        }
        if (Input.GetKey(KeyCode.B)) {
            rx.ChangeState("st-2");
        }
        if (Input.GetKey(KeyCode.E)) {
            //エラーを発生させてみる
            rx.Abort(new Exception("An error occurred"));
        }
        if (Input.GetKey(KeyCode.Q)) {
            rx.Complete();
        }
    }
}
```

## 目的

UniRxが分からない人にFromCoroutineを使ってもらう用（エラー遷移の記述が楽になるので）
