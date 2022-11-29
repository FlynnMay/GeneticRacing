using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

internal static class JsonHelper
{
    public static string ArrayToJson<T>(T[] array, bool prettyPrint = false)
    {
        return JsonUtility.ToJson(new ArrayWrapper<T>(array), prettyPrint);
    }

    public static T[] ArrayFromJson<T>(string json)
    {
        ArrayWrapper<T> wrapper = JsonUtility.FromJson<ArrayWrapper<T>>(json);

        if (wrapper == null)
            return new T[0];

        return wrapper.array;
    }

    private class ArrayWrapper<T>
    {
        public T[] array;
        public ArrayWrapper(T[] array)
        {
            this.array = array;
        }
    }
}

