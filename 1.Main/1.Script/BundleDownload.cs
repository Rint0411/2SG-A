using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;

public class BundleDownload : MonoBehaviour
{
    public string sceneToLoad; // Addressables���� �ҷ��� �� �̸�
    public string label = "Scene"; // �ٿ�ε��� ���� ���̺�

    private AsyncOperationHandle<SceneInstance> handle;

    public void Down()
    {
        // �� ���� �� �Ǵ� ��ư ������ �ٿ�ε� ����
        StartCoroutine(DownloadAndPrepareScene());
    }

    private System.Collections.IEnumerator DownloadAndPrepareScene()
    {
        Debug.Log("�� ���� �ٿ�ε� ����...");

        var downloadHandle = Addressables.DownloadDependenciesAsync(label);

        yield return downloadHandle;

        if (downloadHandle.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log("�ٿ�ε� �Ϸ�! ���� StartLoading���� �� �ε� ����.");
            //StartLoading();
        }
        else
        {
            Debug.LogError("���� �ٿ�ε� ����!");
        }
    }





    public void StartLoading()
    {
        StartCoroutine(LoadSceneAsync(sceneToLoad));
    }

    private System.Collections.IEnumerator LoadSceneAsync(string sceneName)
    {
        handle = Addressables.LoadSceneAsync(sceneName, LoadSceneMode.Single);

        while (!handle.IsDone)
        {
            float percent = handle.PercentComplete * 100f;
            //loadingText.text = $"Loading... {percent:F0}%";
            yield return null; // ���� �����ӱ��� ��ٸ�
        }

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            //loadingText.text = "Load Complete!";
            Debug.Log("�� �ε� �Ϸ�!");
        }
        else
        {
            //loadingText.text = "Failed to load scene!";
            Debug.LogError("�� �ε� ����");
        }
    }
}
