using UnityEngine;

public class FollowTransform : MonoBehaviour
{
    [SerializeField] private Transform _toFollow;

    // Update is called once per frame
    void Update()
    {
        if (_toFollow) transform.position = _toFollow.position;
    }

    public void SetToFollow(Transform toFollow)
    {
        _toFollow = toFollow;
    }
}