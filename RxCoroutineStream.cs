using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

/**
 * UniRxのFromCoroutineをUniRxを隠蔽して使えないか実験
 * これだとMonoBehaviourからしか呼べない
 */
public class RxCoroutineStream
{
    private Func<IEnumerator> _customCoroutine;
    private Coroutine _mainTask;
    private IObserver<string> _observer;
    private int _step;
    private string _state = "";

    public RxCoroutineStream()
    {
    }

    public RxCoroutineStream Publish(Coroutine task/*Func<IEnumerator> userCoroutine*/)
    {
        _mainTask = task;
        //_customCoroutine = userCoroutine;
        return this;
    }

    public void Subscribe(Action onComplete)
    {
        Subscribe(null, onComplete, null);
    }

    public void Subscribe(Action<string> onStatus, Action onCompleteAll, Action<Exception> onError)
    {
        _step = 0;
        Observable.FromCoroutine<string>(a => entryCoroutine(a))
        .Subscribe(
            no => { //if (onStartStep != null) {
                onStatus(no);
                _state = no;
                //    } 
            },
            ex => { onError(ex); },
            () => { onCompleteAll(); }
        );

    }

    private IEnumerator entryCoroutine(IObserver<string> observer)
    {
        _observer = observer;
        _observer.OnNext(""); //最初は無条件にstate""
        yield return _mainTask;
    }

    /**
     * 次のステップ
     */
    public RxCoroutineStream Next()
    {
        _observer.OnNext("");
        return this;
    }

    public RxCoroutineStream ChangeState(string state)
    {
        _observer.OnNext(state);
        return this;
    }

    /**
     * 完了
     */
    public RxCoroutineStream Complete()
    {
        _observer.OnCompleted();
        return this;
    }

    /**
     * エラー
     */
    public RxCoroutineStream Abort(Exception error = null)
    {
        _observer.OnError(error);
        return this;
    }

    /**
     * 指定のステップ番号まで待つ
     */
    public IEnumerator WaitStep(int stepNo)
    {
        if(stepNo < _step) {
            return null;
        }
        return new WaitUntil(() => _step == stepNo);
    }

    public IEnumerator WaitState(string aState)
    {
        return new WaitUntil(() => _state == aState);
    }
}