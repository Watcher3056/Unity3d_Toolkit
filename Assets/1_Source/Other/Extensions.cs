using TeamAlpha.Source;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using DG.Tweening;
using System.Collections;
using DG.Tweening.Core;
using System.Reflection;
using Animancer;
using Cinemachine;

namespace TeamAlpha.Source
{
    public static partial class Functions
    {
        public static string GetFullName(this GameObject go)
        {
            string name = go.name;
            while (go.transform.parent != null)
            {

                go = go.transform.parent.gameObject;
                name = go.name + "/" + name;
            }
            return name;
        }
        public static void RemoveNull<T>(List<T> list) where T : class
        {
            for (int i = 0; i < list.Count; i++)
                if (list[i] == null)
                {
                    list.RemoveAt(i);
                    i--;
                }
        }
        public static void SetActiveAllComponents(this GameObject content, bool active)
        {
            List<Behaviour> components = new List<Behaviour>(content.GetComponentsInChildren<Behaviour>(true));
            components.AddRange(content.GetComponents<Behaviour>());
            for (int i = 0; i < components.Count; i++)
                components[i].enabled = active;
        }
        public static void ChangeLayerForAll(this IEnumerable<SpriteRenderer> renders, int increment)
        {
            foreach (SpriteRenderer renderer in renders)
                renderer.sortingOrder += increment;
        }
        public static void ChangeLayerIncludingChildrens(this GameObject go, int layer)
        {
            go.layer = layer;
            foreach (Transform child in go.transform)
            {
                child.gameObject.layer = layer;
                if (child.childCount > 0)
                    ChangeLayerIncludingChildrens(child.gameObject, layer);
            }
        }
        public static List<T> GetAllComponents<T>(this GameObject obj, bool includeInactive)
        {
            List<T> result = new List<T>();
            result.AddRange(obj.GetComponents<T>());

            for (int i = 0; i < obj.transform.childCount; i++)
                result.AddRange(obj.transform.GetChild(i).gameObject.GetAllComponents<T>(includeInactive));
            if (!includeInactive)
                result.RemoveAll(r => !(r as UnityEngine.Component).gameObject.activeInHierarchy);

            return result;
        }
        public static T FindNearest<T>(this Transform obj, List<T> objects, float maxSearchDistance = float.MaxValue, params T[] exclude) where T : MonoBehaviour
        {
            return FindNearestFromPoint(obj.position, objects, maxSearchDistance, exclude).GetComponent<T>();
        }
        public static T FindNearestFromPoint<T>(this Vector3 pos, List<T> objects, float maxSearchDistance = float.MaxValue, params T[] exclude) where T : MonoBehaviour
        {
            List<Transform> listExclude = new List<Transform>(exclude.Length);
            List<Transform> _objects = new List<Transform>(objects.Count);
            foreach (MonoBehaviour mb in exclude)
                listExclude.Add(mb.transform);
            foreach (MonoBehaviour mb in objects)
                _objects.Add(mb.transform);


            return FindNearestFromPoint(pos, _objects, maxSearchDistance, listExclude.ToArray())?.GetComponent<T>();
        }
        public static Transform FindNearest(this Transform obj, List<Transform> objects, float maxSearchDistance = float.MaxValue, params Transform[] exclude)
        {
            List<Transform> listExclude = new List<Transform>(exclude);
            listExclude.Add(obj);
            return FindNearestFromPoint(obj.position, objects, maxSearchDistance, exclude);
        }
        public static Transform FindNearestFromPoint(this Vector2 pointFrom, List<Transform> objects, float maxSearchDistance = float.MaxValue, params Transform[] exclude)
        {
            return FindNearestFromPoint((Vector3)pointFrom, objects, maxSearchDistance, exclude);
        }
        public static Transform FindNearestFromPoint(this Vector3 pointFrom, List<Transform> objects, float maxSearchDistance = float.MaxValue, params Transform[] exclude)
        {
            List<Transform> listExclude = new List<Transform>(exclude);
            Transform nearestObject = null;
            for (int i = 0; i < objects.Count; i++)
            {
                if (listExclude.Contains(objects[i]))
                    continue;
                if (nearestObject == null)
                    nearestObject = objects[i];
                else if
                    ((Vector3.Distance(pointFrom, objects[i].position) <
                    Vector3.Distance(pointFrom, nearestObject.position)))
                    nearestObject = objects[i];
            }
            if (nearestObject != null &&
                Vector3.Distance(nearestObject.position, pointFrom) > maxSearchDistance)
                return null;
            return nearestObject;
        }
        public static bool IsObjectVisible(this UnityEngine.Camera camera, Renderer renderer)
        {
            return GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(camera), renderer.bounds);
        }
        public static List<T> GetAllPrefabsWithComponent<T>(string path) where T : MonoBehaviour
        {
            List<GameObject> temp = GetAllPrefabs(path);
            List<T> result = new List<T>();
#if UNITY_EDITOR
            for (int i = 0; i < temp.Count; i++)
                if (temp[i].GetComponent<T>() != null)
                    result.Add(temp[i].GetComponent<T>());
#endif
            return result;
        }
        public static List<GameObject> GetAllPrefabs(string path)
        {
            List<GameObject> result = new List<GameObject>();
#if UNITY_EDITOR
            if (!path.Contains(Application.dataPath))
                path = path.Insert(0, Application.dataPath + "/");
            if (path[path.Length - 1] != '/')
                path += '/';
            string[] aFilePaths = Directory.GetFiles(path);

            string[] directoriesInPath = Directory.GetDirectories(path);

            for (int i = 0; i < directoriesInPath.Length; i++)
            {
                Debug.Log("Search in: " + directoriesInPath[i]);
                result.AddRange(GetAllPrefabs(directoriesInPath[i]));
            }
            foreach (string sFilePath in aFilePaths)
            {
                if (!sFilePath.Contains(".prefab") || sFilePath.Contains(".meta"))
                    continue;
                string sAssetPath = sFilePath.Substring(Application.dataPath.Length - 6);

                GameObject objAsset = UnityEditor.AssetDatabase.LoadAssetAtPath(sAssetPath, typeof(GameObject)) as GameObject;

                if (objAsset != null)
                    result.Add(objAsset);
            }
#endif
            return result;
        }
        public static void AddState(this Dictionary<int, StateDefault> statesMap,
            int stateId, Action OnStart = default, Action<int> OnEnd = default)
        {
            StateDefault state = new StateDefault();
            state.OnStart = OnStart;
            state.OnEnd = OnEnd;
            statesMap.Add(stateId, state);
        }
        public static DG.Tweening.Tween DOVolume(this AudioSource audioSource, float endValue, float duration)
        {
            return DG.Tweening.DOTween.To(
                () => audioSource.volume,
                (float value) => audioSource.volume = value,
                endValue, duration);
        }
        public static string AsNumber(this int number)
        {
            string result = number.ToString();
            if (number == 0)
                result += "st";
            else if (number == 1)
                result += "nd";
            else if (number == 2)
                result += "rd";
            else if (number >= 3)
                result += "th";
            return result;
        }
        public static void DestroyAllChilds(this Transform transform, params GameObject[] exclude)
        {
            List<GameObject> excludeList = new List<GameObject>(exclude);

            for (int i = 0; i < transform.childCount; i++)
            {
                GameObject child = transform.GetChild(i).gameObject;
                if (excludeList.Contains(child))
                    continue;

                i--;
                child.transform.SetParent(null);
                if (Application.isPlaying)
                    UnityEngine.Object.Destroy(child);
                else
                    UnityEngine.Object.DestroyImmediate(child);
            }
        }
        public static void LookAtByY(this Transform transform, Vector3 lookAt)
        {
            transform.LookAt(lookAt);
            transform.localEulerAngles += Vector3.right * 90f;
        }
        public static Vector3 ScreenToWorldPoint(this Vector2 input)
        {
            return ScreenToWorldPoint((Vector3)input);
        }
        public static Vector3 WorldToScreenPoint(this Vector2 input)
        {
            return WorldToScreenPoint((Vector3)input);
        }
        public static Vector3 ScreenToWorldPoint(this Vector3 input)
        {
            return CameraManager.Default.cam.ScreenToWorldPoint(input);
        }
        public static Vector3 WorldToScreenPoint(this Vector3 input)
        {
            return CameraManager.Default.cam.WorldToScreenPoint(input);
        }
#if UNITY_EDITOR
        public static void SetHideFlagsRecursively(this GameObject go, HideFlags hideFlags)
        {
            go.hideFlags = hideFlags;
            foreach (Transform child in go.transform)
            {
                SetHideFlagsRecursively(child.gameObject, hideFlags);
            }
        }
        public static string GetAssetPath(this UnityEngine.Object asset)
        {
            GameObject go = null;
            if (asset is GameObject)
                go = asset as GameObject;
            else if (asset is Transform)
                go = (asset as Transform).gameObject;
            else if (asset is MonoBehaviour)
                go = (asset as MonoBehaviour).gameObject;

            //UnityEditor.PrefabInstanceStatus prefabInstanceStatus = 
            //    UnityEditor.PrefabUtility.GetPrefabInstanceStatus(go);
            //bool isPrefabInstance =
            //    prefabInstanceStatus == 
            //    UnityEditor.PrefabInstanceStatus.Connected;

            string assetPath = string.Empty;

            UnityEngine.Object assetToGet = null;
            if (asset is ScriptableObject)
                assetToGet = asset;
            else if (go != null)
                assetToGet = go;
            if (asset is ScriptableObject || (go != null && go.transform.parent == null))
                assetPath = UnityEditor.AssetDatabase.GetAssetPath(assetToGet);
            if (assetPath == string.Empty || assetPath == null)
            {

                if (go != null)
                {
                    try
                    {
                        assetPath =
                            UnityEditor.AssetDatabase.GetAssetPath(
                                UnityEditor.PrefabUtility.GetCorrespondingObjectFromSource(
                                    UnityEditor.PrefabUtility.GetOutermostPrefabInstanceRoot(go)));
                    }
                    catch
                    {

                    }

                    if (assetPath == string.Empty || assetPath == null)
                    {
#if UNITY_2021_1_OR_NEWER
assetPath =
                        UnityEditor.SceneManagement.
                        PrefabStageUtility.GetPrefabStage(go)?.assetPath;
#else
                        assetPath =
                        UnityEditor.Experimental.SceneManagement.
                        PrefabStageUtility.GetPrefabStage(go)?.assetPath;
#endif
                    }
                }
            }
            return assetPath;
        }
#endif
        public static Vector3 GetCanvasScale(this RectTransform transform)
        {
            Vector3 worldScale = transform.localScale;
            Transform parent = transform.parent;

            while (parent != UIManager.Default.mainCanvas.transform)
            {
                worldScale = Vector3.Scale(worldScale, parent.localScale);
                parent = parent.parent;
            }

            return worldScale;
        }
        //public static Vector2 ScreenRectSize(this RectTransform rectTransform)
        //{
        //    Vector3[] worldCorners = new Vector3[4];
        //    rectTransform.GetWorldCorners(worldCorners);

        //    Vector2 result = worldCorners[2].WorldToScreenPoint().Abs() - worldCorners[0].WorldToScreenPoint().Abs();
        //    return result.Abs();
        //}
        public static Vector2 WorldRectSize(this RectTransform rectTransform)
        {
            Vector3[] worldCorners = new Vector3[4];
            rectTransform.GetWorldCorners(worldCorners);

            Vector2 result = worldCorners[2] - worldCorners[0];
            return result;
        }
        public static Vector3[] GetScreenSpaceCorners(this RectTransform rectTransform)
        {
            Vector3[] result = new Vector3[4];
            rectTransform.GetWorldCorners(result);

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = result[i].WorldToScreenPoint();
            }

            return result;
        }
        public static Vector3 Abs(this Vector3 input)
        {
            return new Vector3(Mathf.Abs(input.x), Mathf.Abs(input.y), Mathf.Abs(input.z));
        }
        public static Vector2 Abs(this Vector2 input)
        {
            return new Vector2(Mathf.Abs(input.x), Mathf.Abs(input.y));
        }
        public static Vector3Int Abs(this Vector3Int input)
        {
            return new Vector3Int(Mathf.Abs(input.x), Mathf.Abs(input.y), Mathf.Abs(input.z));
        }
        public static Vector2Int Abs(this Vector2Int input)
        {
            return new Vector2Int(Mathf.Abs(input.x), Mathf.Abs(input.y));
        }
        public static List<T1> Get<T1>(this List<GameObject> input)
        {
            List<T1> result = new List<T1>();

            foreach (GameObject go in input)
            {
                T1 component = go.GetComponent<T1>();
                if (component != null)
                    result.Add(component);
            }
            return result;
        }
        public static List<T2> Get<T1, T2>(this List<T1> input)
            where T1 : UnityEngine.Component where T2 : UnityEngine.Component
        {
            List<T2> result = new List<T2>();

            foreach (T1 item in input)
            {
                T2 component = item.GetComponent<T2>();
                if (component != null)
                    result.Add(component);
            }
            return result;
        }
        public static string FormatNumberAsKilo(this int number)
        {
            return ((float)number).FormatNumberAsKilo();
        }
        public static string FormatNumberAsKilo(this float number)
        {
            if (number >= 1000)
                return decimal.Round((decimal)number / 1000, 1).ToString() + 'k';
            else
                return number.ToString();
        }
        public static T Random<T>(this List<T> list, params T[] exclude)
        {
            List<T> _list = new List<T>(list.ToArray());
            List<T> _listExclude = new List<T>(exclude);

            _list.RemoveAll(e => _listExclude.Contains(e));

            return _list.Random();
        }
        public static T Random<T>(this List<T> list)
        {
            return list[UnityEngine.Random.Range(0, list.Count)];
        }
        public static T Instantiate<T>(this T input, Transform parent = null) where T : Component
        {
            return input.gameObject.Instantiate(parent).GetComponent<T>();
        }
        public static GameObject Instantiate(this GameObject input, Transform parent = null)
        {
            return UnityEngine.Object.Instantiate(input.gameObject, parent);
        }
        public static void DrawBounds(this RectTransform rectTransform)
        {
            Bounds bounds = new Bounds(
                rectTransform.position,
                Vector3.zero);
            Vector3[] corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);

            foreach (Vector3 corner in corners)
                bounds.Encapsulate(corner);
            DebugExtension.DrawBounds(bounds, Color.cyan);
        }
        public static AnimancerState AddAndPlay(
            this AnimancerComponent animancer, AnimationClip clip, float fadeDuration, FadeMode mode = default)
        {
            animancer.States.CreateIfNew(clip);
            return animancer.Play(clip, fadeDuration, mode);
        }
        public static AnimancerState AddAndPlay(
            this AnimancerComponent animancer, ClipTransition clipTransition)
        {
            return animancer.AddAndPlay(clipTransition, clipTransition.FadeDuration, clipTransition.FadeMode);
        }
        public static AnimancerState AddAndPlay(
            this AnimancerComponent animancer, ClipTransition clipTransition, float fadeDuration, FadeMode mode = default)
        {
            animancer.States.CreateIfNew(clipTransition.Clip);
            return animancer.Play(clipTransition, fadeDuration, mode);
        }
        public static void SetAsCurrent(this CinemachineVirtualCamera vcam)
        {
            if (CameraManager.Default.CurActiveVCam != null)
                CameraManager.Default.CurActiveVCam.Priority = int.MaxValue;
            vcam.Priority = int.MinValue;
            //while (CameraManager.Default.CurActiveVCam != vcam)
            //{
            //    CameraManager.Default.CurActiveVCam.Priority = 0;
            //    CameraManager.Default.cameraBrain.ManualUpdate();
            //}
            //vcam.gameObject.SetActive(true);
            //while (CameraManager.Default.CurActiveVCam != null && CameraManager.Default.CurActiveVCam != vcam)
            //{
            //    CameraManager.Default.CurActiveVCam.gameObject.SetActive(false);
            //    CameraManager.Default.cameraBrain.ManualUpdate();
            //}
        }
        public static float Duration(this ClipTransition transition)
        {
            float result = transition.MaximumDuration;
            result -= transition.NormalizedStartTime * transition.MaximumDuration;
            result -= (1f - transition.SerializedEvents.GetNormalizedEndTime()) * transition.MaximumDuration;

            return result;
        }
        public static T GetTarget<T>(this WeakReference<T> weakReference) where T : class
        {
            T result;
            weakReference.TryGetTarget(out result);
            return result;
        }
        public static char GetPositiveOrNegativeSign(this float value, bool inverse = false)
        {
            if ((value > 0 && !inverse) || (value < 0 && inverse))
                return '+';
            else
                return '-';
        }
        public static float GetPositiveOrNegativeSign(this float input) => input >= 0f ? 1 : -1;
#if UNITY_EDITOR
        public static string ToProjectPathFormat(this string path)
        {
            string result = path.Substring(path.IndexOf("Assets"));

            return result;
        }
#endif
    }
}
