using UnityEngine;

public interface ISaveable<T>
{
    T CaptureState();          // Save data
    void RestoreState(T data); // Load data
}
