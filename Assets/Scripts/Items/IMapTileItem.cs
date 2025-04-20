namespace DefaultNamespace
{
    using UnityEngine.Tilemaps;

    public interface IMapTileItem
    {
        void SetTilemaps(Tilemap ground, Tilemap obstacle);
    }
}