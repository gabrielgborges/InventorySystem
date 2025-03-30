using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectCleaner : MonoBehaviour
{
    //Object pooling will be used in the future, as already implemented as a Service -> ObjectPoolingService
    // Only requiring configuration
    private void OnTriggerEnter(Collider other)
    {
        Destroy(other.gameObject);
    }
}
