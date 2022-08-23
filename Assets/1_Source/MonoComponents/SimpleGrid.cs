using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace TeamAlpha.Source
{
#if UNITY_EDITOR
    [ExecuteInEditMode]
#endif
    [RequireComponent(typeof(RectTransform))]
    public class SimpleGrid : MonoBehaviour
    {
        public struct ItemPosition
        {
            public int column;
            public int row;
            public int page;
            public int index;
            public Vector3 anchorPos;
        }

        [FormerlySerializedAs("scaleContentToBounds")]
        public bool keepContentInBounds;
        [Range(0f, 1f), ShowIf("@keepContentInBounds && !keepOriginalCellScale"), BoxGroup("Content Bounds")]
        [LabelText("Scale Factor")]
        public float keepContentInBoundsScaleFactor = 1f;
        [Range(0f, 1f), ShowIf("keepContentInBounds"), BoxGroup("Content Bounds")]
        [LabelText("Offset Factor")]
        public float keepContentInBoundsOffsetFactor = 1f;
        [FormerlySerializedAs("scaleByFixedGridSize"), ShowIf("keepContentInBounds"), BoxGroup("Content Bounds")]
        [LabelText("Keep By Fixed Grid Size")]
        public bool keepContentInBoundsByFixedGridSize;
        public bool centerContentByX;
        public bool centerContentByY;
        [ShowIf("@centerContentByY || centerContentByX")]
        public bool centerByFixedGridSize;
        [HideIf("keepOriginalCellScale")]
        public float cellScale = 1f;
        public bool keepOriginalCellScale;
        public bool keepOriginalCellSize;
        public Vector2 cellSize = new Vector2(100f, 100f);
        public Vector2 anchorPoint = new Vector2(0, 1f);
        public Vector2 cellsPivot = new Vector2(0.5f, 0.5f);
        public Vector2Int gridSize = new Vector2Int(1, 1);
        public Vector2 offset;
        public Vector2 offsetByCell;
        public bool limitOffset;
        public bool multiPaged;
        [ShowIf("multiPaged"), InlineProperty]
        public Vector2 offsetByPage;

        //public Vector2 GapBetweenCells => (offsetByCell.Abs() - cellSize.Abs()) * PrefferedCellScale;
        public int ItemsPerPage => gridSize.x * gridSize.y;
        public int CurColumnsUsed => Mathf.Clamp(transform.childCount, 0, gridSize.x);
        public int CurRowsUsed => Mathf.Clamp(Mathf.CeilToInt(transform.childCount / (float)gridSize.x), 0, gridSize.y);
        public bool UpdatePositions { get => updatePositions; set => updatePositions = value; }
        private bool updatePositions = true;
        public bool UpdateCellSize { get => updateCellSize; set => updateCellSize = value; }
        private bool updateCellSize = true;
        public Vector2 StartPoint
        {
            get
            {
                Vector2 result = Vector2.zero;
                result.x += offset.x;
                result.y -= offset.y;

                return result;
            }
        }
        public Vector2Int GapsAmount
        {
            get
            {
                Vector2Int result = new Vector2Int(CurColumnsUsed, CurRowsUsed);

                if (CurColumnsUsed > 0)
                    result.x--;
                if (CurRowsUsed > 0)
                    result.y--;

                return result;
            }
        }
        public Vector2Int GapsAmountFixed
        {
            get
            {
                Vector2Int result = gridSize;

                if (gridSize.x > 0)
                    result.x--;
                if (gridSize.y > 0)
                    result.y--;

                return result;
            }
        }
        public Vector2 OffsetByCellPreffered
        {
            get
            {
                Vector2 result = Vector2.Lerp(Vector2.zero, offsetByCell, keepContentInBoundsOffsetFactor);
                if (keepContentInBounds)
                {
                    Vector2 freeSpace = Vector2.zero;
                    if (keepContentInBoundsByFixedGridSize)
                        freeSpace = RectTransform.rect.size - ContentRectSizeFixed;
                    else
                        freeSpace = RectTransform.rect.size - ContentRectSize;

                    Vector2Int gapsAmount = Vector2Int.zero;
                    if (keepContentInBoundsByFixedGridSize)
                        gapsAmount = GapsAmountFixed;
                    else
                        gapsAmount = GapsAmount;
                    Vector2 spaceOccupiedByOffset = gapsAmount * offsetByCell;
                    Vector2 prefferedOccupation = spaceOccupiedByOffset + freeSpace;
                    Vector2 divider = spaceOccupiedByOffset;
                    if (divider.x == 0f)
                        divider.x = 1f;
                    if (divider.y == 0f)
                        divider.y = 1f;
                    Vector2 diffFactor = (prefferedOccupation / divider).Abs();
                    float factor = Mathf.Min(diffFactor.x, diffFactor.y);
                    factor = Mathf.Lerp(1f, factor, keepContentInBoundsOffsetFactor);
                    if (limitOffset)
                        factor = Mathf.Clamp01(factor);
                    result = factor * offsetByCell;
                }

                return result;
            }
        }
        public Vector2 ContentRectSizeFixed
        {
            get
            {
                Vector2 result = Vector2.one;
                result.x = gridSize.x * offsetByCell.x * cellScale;
                result.y = gridSize.y * offsetByCell.y * cellScale;

                return result.Abs();
            }
        }
        public Vector2 ContentRectSize
        {
            get
            {
                Vector2 result = Vector2.one;
                result.x = CurColumnsUsed * offsetByCell.x * cellScale;
                result.y = CurRowsUsed * offsetByCell.y * cellScale;

                return result.Abs();
            }
        }
        public float PrefferedCellScale
        {
            get
            {
                float result = cellScale;
                if (keepContentInBounds)
                {
                    Vector2 _gridSize = Vector2.zero;
                    if (keepContentInBoundsByFixedGridSize)
                        _gridSize = gridSize;
                    else
                        _gridSize = new Vector2(CurColumnsUsed, CurRowsUsed);

                    Vector2 sizeDiff = Vector2.zero;
                    if (keepContentInBoundsByFixedGridSize)
                        sizeDiff = ContentRectSizeFixed - RectTransform.rect.size;
                    else
                        sizeDiff = ContentRectSize - RectTransform.rect.size;

                    Vector2 spaceOccupiedByCells = _gridSize * cellSize;
                    Vector2 prefferedOccupation = spaceOccupiedByCells - sizeDiff;
                    Vector2 diffFactor = (prefferedOccupation / spaceOccupiedByCells).Abs();
                    float factor = Mathf.Min(diffFactor.x, diffFactor.y);
                    factor = Mathf.Lerp(1f, factor, keepContentInBoundsScaleFactor);

                    result = Mathf.Clamp(factor * cellScale, 0f, cellScale);
                }
                return result;
            }
        }
        //public Vector2 AnchorPoint
        //{
        //    get
        //    {
        //        Vector2 rectSize = localRect.rect.size;
        //        Vector2 result = new Vector2();
        //        result.x = Mathf.Lerp(-l)
        //        result *= localRect.rect.size / 2f;

        //        return result;
        //    }
        //}
        private bool wasUpdated, delayedUpdate;
        public RectTransform RectTransform => localRect;
        [SerializeField]
        private RectTransform localRect;

        private void Awake()
        {
            localRect = GetComponent<RectTransform>();
        }
        private void Update()
        {
            wasUpdated = false;
#if UNITY_EDITOR
            if (!Application.isPlaying)
                UpdateView();
#endif
        }
        private void LateUpdate()
        {
            if (delayedUpdate)
            {
                delayedUpdate = false;
                UpdateView();
            }
        }
        [Button]
        public void UpdateView()
        {
            if (wasUpdated || !gameObject.activeSelf)
                return;

            RectTransform rt = null;

            int cellsActive = 0;
            for (int i = 0; i < transform.childCount; i++)
            {
                rt = transform.GetChild(i).GetComponent<RectTransform>();
                if (!rt.gameObject.activeSelf)
                {
                    continue;
                }

                if (UpdatePositions || !Application.isPlaying)
                {
                    ItemPosition newPos = GetPositionInMatrixByIndex(cellsActive);

                    ApplyAnchor(rt);
                    rt.anchoredPosition = newPos.anchorPos;
                }
                if (UpdateCellSize || !Application.isPlaying)
                {
                    if (!keepOriginalCellSize)
                        rt.sizeDelta = cellSize;
                    if (!keepOriginalCellScale)
                        rt.localScale = Vector3.one * PrefferedCellScale;
                }

                cellsActive++;
            }
            wasUpdated = true;
        }
        public void ApplyAnchor(RectTransform rectTransform)
        {
            rectTransform.anchorMax = anchorPoint;
            rectTransform.anchorMin = anchorPoint;
            rectTransform.pivot = cellsPivot;
        }
        public void UpdateViewDelayed()
        {
            delayedUpdate = true;
        }
        public ItemPosition GetPositionInMatrixByIndex(int index)
        {
            ItemPosition result = new ItemPosition();
            result.row = index / gridSize.x;
            result.column = index % gridSize.x;
            result.page = index / ItemsPerPage;
            result.index = GetIndex(result.column, result.row);
            result.anchorPos = GetPosition(result.column, result.row);

            return result;
        }
        public Vector3 GetPosition(int column, int row)
        {
            Vector3 result = new Vector3();
            result.x = StartPoint.x + column * OffsetByCellPreffered.x;
            result.y = StartPoint.y - row * OffsetByCellPreffered.y;

            int index = GetIndex(column, row);
            int page = GetPageNumberByIndex(index);
            if (multiPaged)
                result += (Vector3)offsetByPage * page;
            if (centerContentByX)
            {
                Vector2 offset = new Vector2();
                offset.x = localRect.rect.width;
                if (centerByFixedGridSize)
                    offset.x -= GapsAmountFixed.x * OffsetByCellPreffered.x;
                else
                    offset.x -= GapsAmount.x * OffsetByCellPreffered.x;
                offset.x /= 2f;
                result += (Vector3)offset;
            }
            if (centerContentByY)
            {
                Vector2 offset = new Vector2();
                offset.y = localRect.rect.height;
                if (centerByFixedGridSize)
                    offset.y += GapsAmountFixed.y * OffsetByCellPreffered.y;
                else
                    offset.y += GapsAmount.y * OffsetByCellPreffered.y;
                offset.y /= 2f;
                result += (Vector3)offset;
            }

            return result;
        }
        public int GetPageNumberByIndex(int index)
        {
            return index / ItemsPerPage;
        }
        public int GetIndex(int column, int row)
        {
            return row * gridSize.x + column;
        }
        public RectTransform GetChildByGridPos(int column, int row)
        {
            int index = GetIndex(column, row);
            if (index < transform.childCount)
                return transform.GetChild(index) as RectTransform;
            else
                return null;
        }
        private void OnDrawGizmosSelected()
        {
            localRect.DrawBounds();
        }
    }
}
