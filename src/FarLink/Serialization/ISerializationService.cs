﻿using System;
using System.Net.Mime;

namespace FarLink.Serialization
{
    public interface ISerializationService
    {
        Serialized Serialize(object value, ContentType contentType, params ContentType[] alternativeContentTypes);
        object Deserialize(Serialized data, Type awaitedType, params Type[] alternativeTypes);
    }
}