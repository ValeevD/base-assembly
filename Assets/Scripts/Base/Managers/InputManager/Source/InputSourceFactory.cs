using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Base
{
    public class InputSourceFactory : AbstractService<IInputSourceFactory>, IInputSourceFactory
    {

        public int poolSize = 32;
        public InputSource inputSourcePrefab;

        public Transform sourceGroup;

        public IInputSource Spawn()
        {
            InputSource newSource;

            if(inputSourcePrefab != null)
            {
                newSource = Instantiate(inputSourcePrefab);
                newSource.transform.parent = sourceGroup;
            }
            else
            {
                var go = new GameObject();
                go.transform.parent = sourceGroup;
                newSource = go.AddComponent<InputSource>();
            }

            newSource.gameObject.SetActive(true);
            return newSource;
        }

        public void Despawn(IInputSource source)
        {
            InputSource inputSource = source as InputSource;

            inputSource.gameObject.SetActive(false);
            inputSource.DisconnectAllActions();
            Destroy(inputSource.gameObject);
        }
    }
}
