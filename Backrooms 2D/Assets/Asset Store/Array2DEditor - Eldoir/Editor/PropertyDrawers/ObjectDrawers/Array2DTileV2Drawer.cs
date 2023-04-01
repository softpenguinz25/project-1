using UnityEngine;
using UnityEditor;

namespace Array2DEditor
{
    [CustomPropertyDrawer(typeof(Array2DTileV2Inspector))]
    public class Array2DTileV2InspectorDrawer : Array2DObjectDrawer<GroupTileV2Data>
    {
        protected override Vector2Int GetDefaultCellSizeValue() => new Vector2Int(96, 16);
    }
}
