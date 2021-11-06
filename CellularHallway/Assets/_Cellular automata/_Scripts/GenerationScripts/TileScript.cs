using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileScript : MonoBehaviour
{
    public FloorTile tileTextures;
    public void SetSprite(int TextureIndex)
    {
        if(TextureIndex < tileTextures.textures.Length)
        {
            GetComponent<SpriteRenderer>().sprite = tileTextures.textures[TextureIndex];
            if(TextureIndex == 0 || TextureIndex == 1)
            {
                Destroy(GetComponent<BoxCollider2D>());
            }
        }
    }
}
