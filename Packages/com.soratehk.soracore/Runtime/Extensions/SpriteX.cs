using UnityEngine;

namespace SoraTehk.Extensions {
    public static partial class SpriteX {
        public static Sprite CreateRandomColor(int width, int height) {
            return Sprite.Create(
                Texture2DX.CreateRandomColor(width, height),
                new Rect(0, 0, width, height),
                new Vector2(0.5f, 0.5f)
            );
        }
    }
}