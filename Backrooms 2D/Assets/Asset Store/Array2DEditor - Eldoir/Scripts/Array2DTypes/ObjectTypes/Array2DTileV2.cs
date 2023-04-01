using UnityEngine;

namespace Array2DEditor
{
    [System.Serializable]
    public class Array2DTileV2Inspector : Array2D<GroupTileV2Data>
    {
        [SerializeField]
        CellRowTileV2Inspector[] cells = new CellRowTileV2Inspector[Consts.defaultGridSize];

        protected override CellRow<GroupTileV2Data> GetCellRow(int idx)
        {
            return cells[idx];
        }
    }
}