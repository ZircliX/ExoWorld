using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;

namespace OverBang.GameName.Core.Scene
{
    public static class SceneLoader
    {
        public static Task LoadSceneAsync(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            AsyncOperation op = UnitySceneManager.LoadSceneAsync(sceneName, mode);

            if (op != null) 
                op.completed += _ => tcs.SetResult(true);

            return tcs.Task;
        }
    }
}