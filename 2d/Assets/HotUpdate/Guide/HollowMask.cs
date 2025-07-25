using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HollowMask : Graphic,ICanvasRaycastFilter, IPointerClickHandler
{
    
    // public static HollowMask Instance;
    
    //public RectTransform clickTipsEff;
    public RectTransform target;
    public RectTransform follow;
    public bool skipFilter;

    private Vector2? lastPosition = null; // 保存上一次的位置

    // private TweenerCore<float, float, FloatOptions> tweenerCore;

    protected new void Awake()
    {
        base.Awake();
        // Instance = this;
    }

    public void Play(RectTransform pointTarget)
    {
        _clickCount = 0;
        this.target = pointTarget;
        this.material.renderQueue = 4001;
    }

    public void ResetRenderQueue()
    {
        this.material.renderQueue = 2000;
    }

    // private void Update()
    // {
    //     if (target == null)
    //     {
    //         return;
    //     }
    //     
    //     // 计算新位置
    //
    //     // 将目标 UI 的局部坐标中心点（RectTransform.rect.center）转换为世界坐标
    //     var worldPos = target.TransformPoint(target.rect.center);
    //
    //     // 将世界坐标转换为屏幕坐标（屏幕左下角为原点）
    //     var scPos = RectTransformUtility.WorldToScreenPoint(UIModule.Instance.UICamera, worldPos);
    //
    //     // 将屏幕坐标转换为 rectTransform 的局部坐标
    //     RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, scPos, UIModule.Instance.UICamera, out var newScPos);
    //
    //     // 如果与上次的位置相同，则不创建新的 Tweener
    //     if (lastPosition.HasValue && Vector2.Distance(lastPosition.Value, newScPos) < Mathf.Epsilon)
    //     {
    //         //Debug.Log("位置未变化，跳过动画刷新");
    //         return;
    //     }
    //
    //     // Debug.Log("显示遮罩!");
    //
    //     // 如果位置不同，先结束之前的动画
    //     tweenerCore?.Kill();
    //
    //     // 更新 lastPosition 为当前的位置
    //     lastPosition = newScPos;
    //
    //     // 更新位置和动画
    //     follow.position = newScPos;
    //     material.SetFloat("_MaskType", 0);
    //
    //     Vector4 vector4 = new Vector4(newScPos.x, newScPos.y, 0, 0);
    //     float radius = 500;
    //
    //     // tweenerCore = DOTween.To(() => radius, r =>
    //     // {
    //     //     vector4.z = r;
    //     //     material.SetVector("_Origin", vector4);
    //     // }, 150, 1.2f).OnComplete(() => {
    //     //     //clickTipsEff.gameObject.SetActive(false);
    //     // });
    //
    //     tweenerCore.SetUpdate(true);
    //
    // }

    // public void ShowOld()
    // {
    //     if (target == null)
    //         return; // 异常情况 目标按钮消失
    //     
    //     // 计算新位置
    //     var worldPos = target.TransformPoint(target.rect.center);
    //     var scPos = RectTransformUtility.WorldToScreenPoint(UIModule.Instance.UICamera, worldPos);
    //     RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, scPos, UIModule.Instance.UICamera, out var newScPos);
    //
    //     tweenerCore?.Kill();
    //
    //     // 更新 lastPosition 为当前的位置
    //     lastPosition = newScPos;
    //
    //     // 更新位置和动画
    //     follow.anchoredPosition = newScPos;
    //     material.SetFloat("_MaskType",0);
    //
    //     Vector4 vector4 = new Vector4(newScPos.x,newScPos.y,0,0);
    //     float radius = 500;
    //
    //     tweenerCore = DOTween.To(() => radius, r =>
    //     {
    //         vector4.z = r;
    //         material.SetVector("_Origin", vector4);
    //     }, 150, 1.2f).OnComplete(() =>{
    //         //clickTipsEff.gameObject.SetActive(false);
    //     });
    //
    //     tweenerCore.SetUpdate(true);
    //
    // }


    public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
    {
        if (skipFilter)
        {
            return true;
        }

        if (target == null ||
            !target.gameObject.activeInHierarchy) 
        {
            return true;
        }
        
        bool contains = RectTransformUtility.RectangleContainsScreenPoint(target, sp, eventCamera);
        
        
        // Debug.Log(contains+"判断raycast valid" + Environment.StackTrace);
        return !contains;
    }

    private int _clickCount = 0;
    public Action skipRequest;
    public void OnPointerClick(PointerEventData eventData)
    {
        // bool contains = RectTransformUtility.RectangleContainsScreenPoint(target,eventData.position,UIModule.Instance.UICamera);
        // if (!contains)
        // {
        //     ShowOld();
        // }
        _clickCount++;
        if (_clickCount >= 8)
        {
            _clickCount = 0;
            skipRequest?.Invoke();
        }
    }
}
