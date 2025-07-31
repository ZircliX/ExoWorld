using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace DeadLink.Gravity.Implementations
{
    [AddComponentMenu("RogueLike/Gravity/Mesh")]
    public class MeshGravityZone : GravityZone
    {
        [SerializeField] private MeshFilter meshFilter;
        
        protected override Vector3 GetGravityForReceiver(GravityReceiver receiver)
        { 
            Vector3 receiverPosition = meshFilter.transform.InverseTransformPoint(receiver.Position);
            
            using (Mesh.MeshDataArray data = Mesh.AcquireReadOnlyMeshData(meshFilter.mesh))
            {
                Mesh.MeshData currentData = data[0];
                int currentDataVertexCount = currentData.vertexCount;
                
                using (NativeArray<Vector3> vertices =
                       new NativeArray<Vector3>(currentDataVertexCount, Allocator.TempJob))
                using (NativeArray<Vector3> normals =
                       new NativeArray<Vector3>(currentDataVertexCount, Allocator.TempJob))
                {
                    currentData.GetNormals(normals);
                    currentData.GetVertices(vertices);

                    float lastDistance = Mathf.Infinity;
                    Vector3 normal = Vector3.zero;
                    
                    for (int i = 0; i < currentData.subMeshCount; i++)
                    {
                        SubMeshDescriptor currentSubMesh = currentData.GetSubMesh(i);
                        
                        using (NativeArray<ushort> triangles =
                               new NativeArray<ushort>(currentSubMesh.indexCount, Allocator.TempJob))
                        {
                            currentData.GetIndices(triangles, 0);
                            
                            for (int j = currentSubMesh.indexStart; j < currentSubMesh.indexStart + currentSubMesh.indexCount; j += 3)
                            {
                                ushort aIndex = triangles[j];
                                ushort bIndex = triangles[j + 1];
                                ushort cIndex = triangles[j + 2];
                                
                                Vector3 a = vertices[aIndex];
                                Vector3 b = vertices[bIndex];
                                Vector3 c =  vertices[cIndex];
                                
                                Vector3 center = (a + b + c) / 3;
                                
                                float distance = (receiverPosition - center).sqrMagnitude;

                                if (lastDistance > distance)
                                {
                                    Vector3 na = normals[aIndex];
                                    Vector3 nb = normals[bIndex];
                                    Vector3 nc = normals[cIndex];
                                    
                                    Vector3 tempNormal = (na + nb + nc) / 3;
                                    if (Vector3.Dot(center - receiverPosition, tempNormal) < .1f)
                                    {
                                        normal = tempNormal;
                                        lastDistance = distance;
                                    }
                                }
                            }
                            
                        }
                    }

                    return meshFilter.transform.TransformDirection(-normal);
                }
            }
        }
    }
}