using UnityEngine;

namespace SoraTehk.Extensions {
    // ReSharper disable once InconsistentNaming
    public static partial class Texture2DX {
        public static UObject? FromBytes(byte[] bytes) {
            var tex = new Texture2D(2, 2);
            return tex.LoadImage(bytes) ? tex : null;
        }
        public static Texture2D CreateRandomColor(int width, int height) {
            Color randomColor = new Color(Random.value, Random.value, Random.value);
            Texture2D texture = new Texture2D(width, height);
            Color[] pixels = new Color[width * height];
            
            for (int i = 0; i < pixels.Length; i++) {
                pixels[i] = randomColor;
            }
            
            texture.SetPixels(pixels);
            texture.Apply();
            return texture;
        }
    }
}