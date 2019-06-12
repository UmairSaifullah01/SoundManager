using UnityEngine;
namespace UMGS
{
    public abstract class Singleton<T> : MonoBehaviour where T : Component
    {

        #region Fields

        /// <summary>
        /// The instance.
        /// </summary>
        protected static T instance;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<T> ();
                    if (instance == null)
                    {
                        GameObject obj = new GameObject
                        {
                            name = typeof (T).Name
                        };
                        instance = obj.AddComponent<T> ();
                    }
                }
                return instance;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Use this for initialization.
        /// </summary>
        protected virtual void Awake ()
        {
            if (instance == null)
            {
                instance = this as T;
                DontDestroyOnLoad (gameObject);
            }
            else
            {
                Destroy (gameObject);
            }
        }

        #endregion

    }
    public abstract class SingletonScene<T> : Singleton<T> where T : Component
    {
        protected override void Awake ()
        {
            if (instance == null)
            {
                instance = this as T;
            }
        }
    }
    public abstract class SingletonScenePro<T> : SingletonPro<T> where T : Component
    {
        protected override void Awake ()
        {
            if (instance == null)
            {
                instance = this as T;
            }
        }
    }
    public abstract class SingletonInternal<T> : MonoBehaviour where T : Component
    {
        protected static T instance;
        protected virtual void Awake ()
        {
            if (instance == null)
            {
                instance = this as T;
            }
        }
    }
    public abstract class SingletonPro<T> : ROL where T : Component
    {

        #region Fields

        /// <summary>
        /// The instance.
        /// </summary>
        protected static T instance;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<T> ();
                    if (instance == null)
                    {
                        GameObject obj = new GameObject
                        {
                            name = typeof (T).Name
                        };
                        instance = obj.AddComponent<T> ();
                    }
                }
                return instance;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Use this for initialization.
        /// </summary>
        protected virtual void Awake ()
        {
            if (instance == null)
            {
                instance = this as T;
                DontDestroyOnLoad (gameObject);
            }
            else
            {
                Destroy (gameObject);
            }
        }

        #endregion

    }
    public class ROL : MonoBehaviour
    {

    }
}