using UnityEngine;
using System.Collections;
using System.Threading.Tasks;
using Base;

public class SceneManager : AbstractService<ISceneManager>, ISceneManager
{
    void ISceneManager.LoadScene(string newSceneName)
    {
        StartCoroutine(LoadSceneAsync(newSceneName));
    }

    IEnumerator LoadSceneAsync(string newSceneName){
        Task[] tasks = eventBus.RaiseEventAsync<IOnBeginSceneEnd>(
            (x) => { return x.Do(); }
        );

        Task allTasks = Task.WhenAll(tasks);

        yield return new WaitUntil(() => allTasks.IsCompleted);

        eventBus.RaiseEvent<IOnCurrentSceneUnload>(
            (x) => x.Do()
        );

        AsyncOperation operation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("SampleScene");

        while(!operation.isDone){
            eventBus.RaiseEvent<IOnSceneLoadProgress>(
                (x) => x.Do(operation.progress)
            );
        }

        tasks = eventBus.RaiseEventAsync<IOnEndSceneLoad>(
            (x) => { return x.Do(); }
        );

        allTasks = Task.WhenAll(tasks);

        yield return new WaitUntil(() => allTasks.IsCompleted);
    }
}
