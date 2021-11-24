using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SawBlade : DeathCollider
{
    [SerializeField] private Transform[] _bladePositions;
    [SerializeField] private float _movementSpeed;
    [SerializeField] private float _rotationSpeed;
    
    private int _positionIndex;

    // Start is called before the first frame update
    void Start()
    {
        transform.DORotate(Vector3.forward * -360, 1 / _rotationSpeed, RotateMode.FastBeyond360).SetLoops(-1)
            .SetEase(Ease.Linear).SetRelative(true);

        AudioManager.instance.PlayClipAtGameObject("ElectricSaw01", gameObject, true, 1, 20);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, _bladePositions[_positionIndex].position, _movementSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, _bladePositions[_positionIndex].position) < 0.1f)
        {
            if (_positionIndex == _bladePositions.Length - 1)
            {
                _positionIndex = 0;
            }
            else
            {
                _positionIndex++;
            }
        }
    }
}
