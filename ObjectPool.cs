using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public abstract class ObjectPool<T> {
    static Func<T> _produce;
    static int _capacity;
    static Stack<T> _objects;
    public ObjectPool(Func<T> factoryMethod, int capacity) {
        _produce = factoryMethod;
        _capacity = capacity;
    }

    public static void Reset() {

    }

    public static T GetInstance () {
        T instance;

        if(_objects.Peek() == null) {
            instance = _produce.Invoke();
        } else {
            instance = _objects.Pop();
        }

        return instance;
    }

    public static void Despawn(T instance) {
        _objects.Push(instance);
    }
}