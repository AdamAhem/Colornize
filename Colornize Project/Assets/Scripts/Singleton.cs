using UnityEngine;

public class Singleton : MonoBehaviour {

    private static Singleton instance;

    public static Singleton Instance {
        get {
            if (instance == null) {
                instance = new Singleton();
            }
            return instance;
        }
    }

}