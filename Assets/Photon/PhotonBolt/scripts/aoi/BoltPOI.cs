using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class BoltPOI : Bolt.EntityBehaviour {
  static Mesh mesh;
  static Material poiMat;
  static Material aoiDetectMat;
  static Material aoiReleaseMat;
  static List<BoltPOI> availablePOIs = new List<BoltPOI>();

  public static Mesh Mesh {
    get {
      if (!mesh) {
        mesh = (Mesh)Resources.Load("IcoSphere", typeof(Mesh));
      } 

      return mesh;
    }
  }

  public static Material MaterialPOI {
    get {
      if (!poiMat) {
        poiMat = CreateMaterial(new Color(0f, 37f / 255f, 1f));
      }

      return poiMat;
    }
  }

  public static Material MaterialAOIDetect {
    get {
      if (!aoiDetectMat) {
        aoiDetectMat = CreateMaterial(new Color(37f / 255f, 0.4f, 0f));
      }

      return aoiDetectMat;
    }
  }

  public static Material MaterialAOIRelease {
    get {
      if (!aoiReleaseMat) {
        aoiReleaseMat = CreateMaterial(new Color(1f, 37f / 255f, 0f));
      }

      return aoiReleaseMat;
    }
  }
  static Material CreateMaterial(Color c) {
    Material m = new Material(Resources.Load("BoltShaderPOI", typeof(Shader)) as Shader);
    m.hideFlags = HideFlags.HideAndDontSave;
    m.SetColor("_SpecColor", c);
    return m;
  }

  [SerializeField]
  public float radius = 16f;

  void Update() {
    Graphics.DrawMesh(BoltPOI.Mesh, Matrix4x4.TRS(transform.position, Quaternion.identity, new Vector3(radius, radius, radius)), BoltPOI.MaterialPOI, 0);
  }

  void OnDestroy() {
    availablePOIs.Remove(this);
  }

  void BoltSceneObject() {
    if (BoltNetwork.IsClient) {
            Destroy(gameObject);
    }
  }

  public override void Attached() {
    availablePOIs.Add(this);
  }

  public override void Detached() {
    availablePOIs.Remove(this);
  }

  public static void UpdateScope(BoltAOI aoi, BoltConnection connection) {
    var aoiPos = aoi.transform.position;
    var aoiDetect = aoi.detectRadius;
    var aoiRelease = aoi.releaseRadius;

    for (int i = 0; i < availablePOIs.Count; ++i) {
      var poi = availablePOIs[i];
      var poiPos = poi.transform.position;
      var poiRadius = poi.radius;

      if (OverlapSphere(aoiPos, poiPos, aoiDetect, poiRadius)) {
        poi.entity.SetScope(connection, true);
      }
      else {
        if (OverlapSphere(aoiPos, poiPos, aoiRelease, poiRadius) == false) {
          poi.entity.SetScope(connection, false);
        }
      }
    }
  }

  static bool OverlapSphere(Vector3 a, Vector3 b, float aRadius, float bRadius) {
    float r = aRadius + bRadius;
    return (a - b).sqrMagnitude <= (r * r);
  }
}
