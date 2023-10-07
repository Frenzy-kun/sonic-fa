using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DynamicLevelLoader : MonoBehaviour
{
    public Transform mCamera;
    public string SceneName = "Island";
    public string ScenePath = "Assets/Resources/Act Skin/Default Zone/Scene/Chunks/";

    private Vector3 lastClick = Vector3.zero;

    public int distance = 1;

    Vector2Int selectedSlot = Vector2Int.zero;
    Vector2Int previousSelectedSlot = Vector2Int.zero;



    private void Update()
    {
        previousSelectedSlot = selectedSlot;
        selectedSlot.x = Mathf.FloorToInt(mCamera.position.x / 2000f) + 20;
        selectedSlot.y = Mathf.FloorToInt(mCamera.position.y / 2000f) + 20;

        //Debug.Log(selectedSlot);

        if (previousSelectedSlot.x != selectedSlot.x || previousSelectedSlot.y != selectedSlot.y)
        {
            ReloadChunks();
        }
    }


    private void ReloadChunks()
    {
        string q = "";
        for (int x = selectedSlot.x -distance -1; x <= selectedSlot.x + distance +1; x++)
        {
            q = "";
            for (int y = selectedSlot.y - distance - 1; y <= selectedSlot.y + distance + 1; y++)
            {
                if (isOnDistance(x,y))
                {
                    q += "O";
                    LoadChunk(x, y);
                }
                else
                {
                    q += "X";
                    UnloadChunk(x, y);
                }
            }
            Debug.Log(q);
        }
        
    }

    public bool isOnDistance(int x, int y)
    {
        return (x >= selectedSlot.x - distance && x <= selectedSlot.x + distance && y >= selectedSlot.y - distance && y <= selectedSlot.y + distance);
    }

    internal void LoadChunk (int x, int y)
    {
        //Debug.Log("LOADING: " + GetChunkSceneName(x, y));
        Scene scene = SceneManager.GetSceneByName(GetChunkSceneName(x, y));
        if (scene.IsValid() && scene.isLoaded)
        {

        }
        else
        {
            try
            {
                SceneManager.LoadSceneAsync(GetChunkSceneName(x, y), LoadSceneMode.Additive);
            }
            catch (Exception e)
            {
                
            }
        }
    }

    internal void UnloadChunk(int x, int y)
    {
        Scene scene = SceneManager.GetSceneByName(GetChunkSceneName(x, y));

        if (scene.isLoaded)
        {
            SceneManager.UnloadSceneAsync(scene);
        }


        //SceneManager.UnloadSceneAsync(GetChunkSceneName(x, y));
        //Debug.Log("UNLOADING: " + GetChunkSceneName(x, y));
    }


    private void OnDrawGizmos()
    {
        Vector3 origin, end;
        Vector3 size = new Vector3(2000f, 2000f, 0.1f);
        Color c = new Color(0.8f, 0.3f, 1f);
        Color transparent = new Color(0f, 0f, 0f, 0.5f);
        float value, value2;
        for (int i = -20; i < 20; i++)
        {
            value = i * 2000f;
            Gizmos.color = c;
            Gizmos.DrawLine(new Vector3(-40000f, value, 0f), new Vector3(40000f, value, 0f));
            Gizmos.color = c;
            Gizmos.DrawLine(new Vector3(value, -40000f , 0f), new Vector3(value, 40000f, 0f));

            

            for (int j = -20; j < 20; j++)
            {
                if (!IsSceneLoaded(j+20,i+20))
                {
                    value2 = j * 2000f;
                    Gizmos.color = transparent;
                    Gizmos.DrawCube(new Vector3(value2 + 1000f, value + 1000f, 0f), size);
                }
                
            }

        }

        /*Gizmos.color = Color.green;
        Gizmos.DrawSphere(lastClick, 100f);*/
        
    }

    public void ProcessClick(Vector2 mouse)
    {
        Vector3 mousePosition = new Vector3(mouse.x, mouse.y, Camera.current.orthographicSize);

        Ray r = UnityEditor.HandleUtility.GUIPointToWorldRay(mouse);

        Camera.current.orthographic = true;
        Vector3 finalPosition = Camera.current.ScreenToWorldPoint(mousePosition);
        lastClick = r.origin;
        lastClick.z = 0f;

        Vector2Int slot = new Vector2Int(Mathf.FloorToInt(lastClick.x / 2000f)+20, Mathf.FloorToInt(lastClick.y / 2000f)+20);
        Debug.Log("Chunk Selected: " + GetChunkSceneName( slot.x, slot.y));

        ChunkSelected(slot);
        
    }

    private bool IsSceneLoaded(int x, int y)
    {
        //Debug.Log("Checking for scene: " + SceneName + "_" + x + "_" + y);

        Scene scene = SceneManager.GetSceneByName(GetChunkSceneName(x,y));

        return  scene.IsValid() && scene.isLoaded;
    }

    public string GetChunkSceneName(int x, int y)
    {
        return SceneName + "_" + x + "_" + y;
    }

    internal void ChunkSelected(Vector2Int slot)
    {
        if (slot.x < 0 || slot.x > 40 || slot.y < 0 || slot.y > 40)
            return;

        if (IsSceneLoaded(slot.x, slot.y))
        {
            Scene scene = SceneManager.GetSceneByName(GetChunkSceneName(slot.x, slot.y));
            UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene);
            Debug.Log(scene.path);
            UnityEditor.SceneManagement.EditorSceneManager.CloseScene(scene, true);
            
        }
        else
        {
            
            Scene scene;
            try
            {
                scene = UnityEditor.SceneManagement.EditorSceneManager.OpenScene(ScenePath + "/" + GetChunkSceneName(slot.x, slot.y) + ".unity", UnityEditor.SceneManagement.OpenSceneMode.Additive);
            }
            catch (Exception e)
            {
                Debug.Log("Scene not found");
                scene = UnityEditor.SceneManagement.EditorSceneManager.NewScene(UnityEditor.SceneManagement.NewSceneSetup.EmptyScene, UnityEditor.SceneManagement.NewSceneMode.Additive);//  (GetChunkSceneName(slot.x, slot.y));
                GameObject go = new GameObject("Content");
                go.AddComponent<LevelRotation>();
                UnityEditor.SceneManagement.EditorSceneManager.MoveGameObjectToScene(go, scene);

                UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene, ScenePath + "/" + GetChunkSceneName(slot.x, slot.y) + ".unity");

                UnityEditor.EditorBuildSettingsScene[] ebs = UnityEditor.EditorBuildSettings.scenes;
                UnityEditor.EditorBuildSettingsScene[] newebs = new UnityEditor.EditorBuildSettingsScene[ebs.Length + 1];
                System.Array.Copy(ebs, newebs, ebs.Length);

                UnityEditor.EditorBuildSettingsScene ebscene = new UnityEditor.EditorBuildSettingsScene(scene.path, true);
                newebs[newebs.Length - 1] = ebscene;

                UnityEditor.EditorBuildSettings.scenes = newebs;


            }
                
            UnityEditor.SceneManagement.EditorSceneManager.SetActiveScene(scene);
        }
    }

    

}
