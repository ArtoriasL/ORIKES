using UnityEngine;
namespace AllenHelper
{
	public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
	{

		protected static T instance;

		/// <summary>
		/// Returns the instance of this singleton.
		/// </summary>
		public static T Instance
		{
			get
			{
				if (instance == null)
				{
					instance = (T)FindObjectOfType(typeof(T));

					if (instance == null)
					{
						GameObject obj = new GameObject(typeof(T).ToString());
						instance = obj.AddComponent<T>();
						//Debug.LogError("An instance of " + typeof(T) + 
						//   " is needed in the scene, but there is none.");
					}
				}
				return instance;
			}
		}
	}
}

