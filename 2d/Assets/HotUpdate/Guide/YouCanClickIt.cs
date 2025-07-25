// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// using Main;
// using UnityEngine;
// using UnityEngine.EventSystems;
//
// namespace HotFix.GameLogic.UI.Common
// {
//     public class YouCanClickIt: Singleton<YouCanClickIt>
//     {
//         public YouCanClickIt()
//         {
//             // UIModule.Instance.OnShowWindow += OnShowWindow;
//             // UIModule.Instance.OnCloseWindow += OnCloseWindow;
//             Utility.Unity.StartCoroutine(CoUpdate());
//         }
//         
//         public enum Priority
//         {
//             Guide,
//             FuncEntry,
//             MainTask,
//             Start,
//         }
//         
//         public class ClickHandler : MonoBehaviour, IPointerClickHandler
//         {
//             public Action<GameObject> OnClick;
//             
//             public void OnPointerClick(PointerEventData eventData)
//             {
//                 if (Input.touchCount > 1)
//                 {
//                     return;
//                 }
//                 OnClick?.Invoke(gameObject);
//             }
//         }
//         
//         private readonly Dictionary<Priority, (Transform, Action<GameObject>, bool)> _requests = new();
//         private Priority? _current;
//         private readonly Dictionary<Priority, HashSet<string>> _hiddenReasons = new();
//
//         private bool _shown = false;
//         
//         private IEnumerator CoUpdate()
//         {
//             while (true)
//             {
//                 yield return null;
//                 if (_current.HasValue && _shown != IsShow(_current.Value))
//                 {
//                     Refresh();
//                 }
//             }
//         }
//
//         public void DebugInfo()
//         {
//             var ins = UIModule.Instance.GetTopWindowIns();
//             bool topIsHome = false;
//             if (ins != null)
//                 topIsHome = ins.GetType() == typeof(UI_HomeWindow) && !UIModule.Instance.IsAnyLoading();
//             Debug.Log($"topIsHome: {topIsHome}");
//             Debug.Log("requests:");
//             foreach (var pair in _requests)
//             {
//                 Debug.Log(pair.Key);
//             }
//
//             Debug.Log("hidden reasons:");
//             foreach (var pair in _hiddenReasons)
//             {
//                 Debug.Log($"{pair.Key}: {pair.Value.PrettyToString()}");
//             }
//             Debug.Log($"can guiding:{GuideMgr.Instance.CanGuiding}");
//         }
//
//         public void AddHiddenReason(Priority priority, string reason)
//         {
//             if (!_hiddenReasons.ContainsKey(priority))
//             {
//                 _hiddenReasons[priority] = new HashSet<string>();
//             }
//             _hiddenReasons[priority].Add(reason);
//         }
//
//         public void RemoveHiddenReason(Priority priority, string reason)
//         {
//             if (_hiddenReasons.ContainsKey(priority) && _hiddenReasons[priority].Contains(reason))
//             {
//                 _hiddenReasons[priority].Remove(reason);
//             }
//         }
//
//         public void Request(Priority priority, string nodePath)
//         {
//             var node = UIModule.UIRootStatic.Find(nodePath).GetComponent<Transform>();
//             Request(priority, node);
//         }
//
//         public void Request(Priority priority, Transform node, bool stayAfterClick = false)
//         {
//             if (_requests.ContainsKey(priority))
//             {
//                 if (_requests[priority].Item1 == node)
//                     return;
//                 var (oldNode, _, _) = _requests[priority];
//                 EnsureInactive(oldNode);
//             }
//             _requests[priority] = (node, null, stayAfterClick);
//             Refresh();
//         }
//
//         public void Request(Priority priority, Transform node, Action<GameObject> onClick, bool stayAfterClick = false)
//         {
//             if (_requests.ContainsKey(priority))
//             {
//                 var (oldNode, oldOnClick, _) = _requests[priority];
//                 if (node == oldNode && onClick == oldOnClick)
//                     return;
//                 
//                 EnsureInactive(oldNode);
//             }
//             _requests[priority] = (node, onClick, stayAfterClick);
//             Refresh();
//         }
//
//         public void CancelRequest(Priority priority)
//         {
//             if (_requests.ContainsKey(priority))
//             {
//                 var (node, _, _) = _requests[priority];
//                 EnsureInactive(node);
//                 _requests.Remove(priority);
//                 Refresh();
//             }
//         }
//
//         public bool HasRequest(Priority priority)
//         {
//             return _requests.ContainsKey(priority);
//         }
//
//         private void EnsureActive(Priority priority)
//         {
//             if (!_requests.ContainsKey(priority))
//                 return;
//             
//             var (node, onClick, stayAfterClick) = _requests[priority];
//             var clickHandler = node.gameObject.EnsureComponent<ClickHandler>();
//             clickHandler.enabled = true;
//             clickHandler.OnClick = go =>
//             {
//                 if (!stayAfterClick)
//                     CancelRequest(priority);
//                 onClick?.Invoke(go);
//             };
//
//             var youCanClickIt = node.Find("YouCanClickIt(Clone)")??node.Find("YouCanClickIt");
//             if (youCanClickIt != null)
//             {
//                 youCanClickIt.gameObject.SetActive(true);
//             }
//             else
//             {
//                 youCanClickIt = AssetManager.Instance.LoadAsset<GameObject>("Assets/GameResources/ResUI/Widget/YouCanClickIt.prefab", node).transform;
//             }
//         }
//
//         private void EnsureInactive(Priority priority)
//         {
//             if (!_requests.ContainsKey(priority))
//                 return;
//             var (node, _, _) = _requests[priority];
//             EnsureInactive(node);
//         }
//
//         private void EnsureInactive(Transform node)
//         {
//             if (node == null)
//             {
//                 return;
//             }
//             
//             var clickHandler = node.GetComponent<ClickHandler>();
//             if (clickHandler != null)
//             {
//                 clickHandler.enabled = false;
//             }
//             var youCanClickIt = node.Find("YouCanClickIt(Clone)")??node.Find("YouCanClickIt");
//             if (youCanClickIt != null)
//             {
//                 youCanClickIt.gameObject.SetActive(false);
//             }
//         }
//
//         private bool IsShow(Priority priority)
//         {
//             // if (priority == Priority.Guide)
//             // {
//             //     return false;
//             // }
//
//             if (!GuideMgr.Instance.CanGuiding)  // fixme can guiding没有回调导致hide
//             {
//                 return false;
//             }
//             
//             if (priority != Priority.Guide)
//             {
//                 var ins = UIModule.Instance.GetTopWindowIns();
//                 bool topIsHome = false;
//                 if (ins != null)
//                     topIsHome = ins.GetType() == typeof(UI_HomeWindow) && !UIModule.Instance.IsAnyLoading();
//                 if (!topIsHome)
//                 {
//                     return false;
//                 }
//             }
//
//             return !_hiddenReasons.ContainsKey(priority) ||  _hiddenReasons[priority].Count <= 0;;
//         }
//
//         public void Refresh()
//         {
//             if (_requests.Count <= 0)
//             {
//                 if (_current.HasValue)
//                 {
//                     EnsureInactive(_current.Value);
//                     _current = null;
//                     _shown = false;
//                 }
//                 return;
//             }
//             var toRemove = new List<Priority>();
//             Priority min = _requests.Keys.First();
//             foreach (var key in _requests.Keys)
//             {
//                 var req = _requests[key];
//                 if (req.Item1 == null)
//                 {
//                     toRemove.Add(key);
//                 }
//                 else
//                 {
//                     min = (Priority)Mathf.Min((int)min, (int)key);
//                 }
//             }
//
//             foreach (var key in toRemove)
//             {
//                 _requests.Remove(key);
//             }
//
//             if (_current.HasValue && _current != min)
//             {
//                 EnsureInactive(_current.Value);
//             }
//
//             _shown = IsShow(min);
//             if (_shown)
//             {
//                 EnsureActive(min);
//             }
//             else
//             {
//                 EnsureInactive(min);
//             }
//             _current = min;
//         }
//     }
// }