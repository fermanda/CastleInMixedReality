using Microsoft.MixedReality.Toolkit.Utilities.Gltf.Schema;
using Microsoft.MixedReality.Toolkit.Utilities.Gltf.Serialization;
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;

public class ModelLoad : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    [Tooltip("The relative asset path to the glTF asset in the Streaming Assets folder.")]
    private string relativePath = "GltfModels/Lantern/glTF/Lantern.gltf";

    /// <summary>
    /// The relative asset path to the glTF asset in the Streaming Assets folder.
    /// </summary>
    public string RelativePath => relativePath;

    /// <summary>
    /// Combines Streaming Assets folder path with RelativePath
    /// </summary>
    public string AbsolutePath => Path.Combine(Path.GetFullPath(Application.streamingAssetsPath), RelativePath);

    [SerializeField]
    [Tooltip("Scale factor to apply on load")]
    private float ScaleFactor;

    [SerializeField]
    public Mesh assignMesh;

    private string dataStr;
    private GameObject meshObject;
    private IEnumerator coroutine;

    private void Awake() {
#if UNITY_ANDROID && !UNITY_EDITOR
        GameObject.Find("HoloCanvas").SetActive(false);
        GameObject.Find("AndroidCanvas").SetActive(true);
#else
        GameObject.Find("HoloCanvas").SetActive(true);
        GameObject.Find("AndroidCanvas").SetActive(false);
#endif
    }
    
    private async void Start()
    {
        Application.targetFrameRate = 60;
        Debug.Log(AbsolutePath);
        Debug.Log(RelativePath);
#if UNITY_ANDROID && !UNITY_EDITOR
        var path = "jar:file://" + Application.dataPath + "!/assets/" + RelativePath;
#else
        var path = AbsolutePath;
#endif
        meshObject = new GameObject();
        meshObject.AddComponent<MeshRenderer>();
        MeshCollider theMesh = meshObject.AddComponent<MeshCollider>();
        theMesh.sharedMesh = assignMesh;
        meshObject.transform.Rotate(-90.0f,0.0f,0.0f);
        Debug.Log("=====>DebugLog : Scene Collider Generated");

        coroutine = GetData(path);
        StartCoroutine(coroutine);
        
    }

    private IEnumerator GetData(string path){
        var loadingRequest = UnityWebRequest.Get(path);
        Debug.Log("=====>DebugLog : " + path);
        loadingRequest.SendWebRequest();
        while (!loadingRequest.isDone) {
            yield return null;
        }
        Debug.Log("=====>DebugLog : Download Complete");
        Debug.Log("=====>DebugLog : data : " + loadingRequest.downloadHandler.data);
        if (loadingRequest.downloadHandler.data != null  && loadingRequest.downloadHandler.data.Length > 0){
            Debug.Log("=====>DebugLog : Data Received");
        }
        else{
            Debug.Log("=====>DebugLog : Data is Empty");
        }
        BuildGLTF(loadingRequest.downloadHandler.data);
        yield break;
    }

    async void BuildGLTF(byte[] dataInput){
        if (dataInput != null  && dataInput.Length > 0){
            Debug.Log("=====>DebugLog : Data is build-able");
        }
        else {
            Debug.Log("=====>DebugLog : Data is Empty. Failed to build");
            return;
        }
        Debug.Log("=====>DebugLog : Building GLTF");
        GltfObject gltfObject = null;

        //gltfObject = await GltfUtility.ImportGltfObjectFromPathAsync(path);
        gltfObject = GltfUtility.GetGltfObjectFromGlb(dataInput);
        Debug.Log("=====>DebugLog : Constructing Model");
        await ConstructGltf.ConstructAsync(gltfObject);

        // Put object in front of user
        Debug.Log("=====>DebugLog : Generating Scene");
        gltfObject.GameObjectReference.transform.position = new Vector3(11.0f, 50.0f, -32.0f);
        gltfObject.GameObjectReference.transform.localScale *= this.ScaleFactor;
        
        
#if UNITY_ANDROID && !UNITY_EDITOR
        meshObject.SetActive(false);
#elif UNITY_EDITOR
        meshObject.transform.position = gltfObject.GameObjectReference.transform.position;
        meshObject.transform.rotation = gltfObject.GameObjectReference.transform.rotation;
        meshObject.transform.localRotation = Quaternion.Euler(-90, 0, 0);
        meshObject.transform.localScale = gltfObject.GameObjectReference.transform.localScale;
        meshObject.SetActive(false);
#endif

#if UNITY_EDITOR
        Debug.Log("=====>DebugLog : Generating User");
        //Rigidbody userBody = userView.AddComponent<Rigidbody>();
        //userBody.constraints = RigidbodyConstraints.FreezeRotation;
#endif

        if (gltfObject != null)
        {
            Debug.Log("Import successful");
        }
    }
}
