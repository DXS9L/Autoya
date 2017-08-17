using System;
using AutoyaFramework.Information;
using UnityEngine;
using UnityEngine.EventSystems;

public class TestReceiver : MonoBehaviour, IUUebViewEventHandler {
    public Action OnLoadStarted;
    public Action OnProgress;
    public Action OnLoaded;
    public Action OnUpdated;
    public Action OnContentLoadFailed;
    public Action OnElementTapped;
    public Action OnElementLongTapped;
    
    void IUUebViewEventHandler.OnLoadStarted() {
        Debug.Log("OnLoadStarted");
        if (OnLoadStarted != null) {
            OnLoadStarted();
        }
    }

    void IUUebViewEventHandler.OnProgress(double progress) {
        Debug.Log("OnProgress progress:" + progress);
        if (OnProgress != null) {
            OnProgress();
        }
    }

    void IUUebViewEventHandler.OnLoaded() {
        Debug.Log("OnLoaded");
        if (OnLoaded != null) {
            OnLoaded();
        }
    }

    void IUUebViewEventHandler.OnUpdated() {
        Debug.Log("OnUpdated");
        if (OnUpdated != null) {
            OnUpdated();
        }
    }

    void IUUebViewEventHandler.OnLoadFailed(ContentType type, int code, string reason) {
        Debug.Log("OnContentLoadFailed type:" + type + " code:" + code + " reason:" + reason);
        if (OnContentLoadFailed != null) {
            OnContentLoadFailed();
        }
    }

    void IUUebViewEventHandler.OnElementTapped(ContentType type, string param, string id) {
        Debug.Log("OnElementTapped type:" + type + " param:" + param + " id:" + id);
        if (OnElementTapped != null) {
            OnElementTapped();
        }
    }

    void IUUebViewEventHandler.OnElementLongTapped(ContentType type, string param, string id) {
        Debug.Log("OnElementLongTapped");
        if (OnElementLongTapped != null) {
            OnElementLongTapped();
        }
    }

    
}
