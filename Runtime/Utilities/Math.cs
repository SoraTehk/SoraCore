using UnityEngine;

namespace SoraCore {
    public static class Math {
        /// <summary>
        /// Translate an angle to a 2D vector (X-axis)
        /// </summary>
        /// <param name="isRadian">Input is radian if true</param>
        public static Vector2 AngleToVector2D(float angle, bool isRadian = false) {
            float rad = isRadian ? angle
                                 : angle * Mathf.Deg2Rad;

            return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
        }

        /// <summary>
        /// Translate a 2D vector to an angle (X-axis)
        /// </summary>
        /// <param name="isRadian">Output as radian if true</param>
        public static float VectorToAngle2D(Vector2 dir, bool isRadian = false) {
            float rad = Mathf.Atan2(dir.y, dir.x);

            return isRadian ? rad
                            : rad * Mathf.Rad2Deg;
        }

        /// <summary>
        /// Offset a 2D <paramref name="origin"/> vector angle by <paramref name="angle"/> (X-axis)
        /// </summary>
        /// <param name="isRadian">Input/output is radian if true</param>
        public static Vector2 ChangeVectorAngle2D(Vector2 origin, float angle, bool isRadian = false) {
            float offsetRad = isRadian ? angle
                                       : angle * Mathf.Rad2Deg;

            float newRad = VectorToAngle2D(origin, isRadian) + offsetRad;

            return AngleToVector2D(newRad, isRadian) * origin.magnitude;
        }

        #region GetRandomPointOnMesh
        /// <summary>
        /// Local normalized
        /// (https://gist.github.com/v21/5378391)
        /// </summary>
        public static Vector3 GetRandomPointOnMesh(Mesh mesh) {
            //if you're repeatedly doing this on a single mesh, you'll likely want to cache cumulativeSizes and total
            float[] sizes = GetTriSizes(mesh.triangles, mesh.vertices);
            float[] cumulativeSizes = new float[sizes.Length];
            float total = 0;

            for (int i = 0; i < sizes.Length; i++) {
                total += sizes[i];
                cumulativeSizes[i] = total;
            }

            //so everything above this point wants to be factored out

            float randomsample = Random.value * total;

            int triIndex = -1;

            for (int i = 0; i < sizes.Length; i++) {
                if (randomsample <= cumulativeSizes[i]) {
                    triIndex = i;
                    break;
                }
            }

            if (triIndex == -1) Debug.LogError("triIndex should never be -1");

            Vector3 a = mesh.vertices[mesh.triangles[triIndex * 3]];
            Vector3 b = mesh.vertices[mesh.triangles[triIndex * 3 + 1]];
            Vector3 c = mesh.vertices[mesh.triangles[triIndex * 3 + 2]];

            //generate random barycentric coordinates

            float r = Random.value;
            float s = Random.value;

            if (r + s >= 1) {
                r = 1 - r;
                s = 1 - s;
            }
            //and then turn them back to a Vector3
            Vector3 pointOnMesh = a + r * (b - a) + s * (c - a);
            return pointOnMesh;

        }
        private static float[] GetTriSizes(int[] tris, Vector3[] verts) {
            int triCount = tris.Length / 3;
            float[] sizes = new float[triCount];
            for (int i = 0; i < triCount; i++) {
                sizes[i] = .5f * Vector3.Cross(verts[tris[i * 3 + 1]] - verts[tris[i * 3]], verts[tris[i * 3 + 2]] - verts[tris[i * 3]]).magnitude;
            }
            return sizes;

            //for (int ii = 0; ii < indices.Length; ii += 3)
            //{
            //    Vector3 A = Points[indices[ii]];
            //    Vector3 B = Points[indices[ii + 1]];
            //    Vector3 C = Points[indices[ii + 2]];
            //    Vector3 V = Vector3.Cross(A - B, A - C);
            //    Area += V.magnitude * 0.5f;
            //}
        }
        #endregion
    }
}