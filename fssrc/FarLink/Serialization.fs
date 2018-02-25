namespace FarLink
open System
open System.Net.Mime

type SerializationError =
    | CannotSerializeType 
    | UnknownContentType
    | InvalidData of Exception
    
type Serializer = obj -> ContentType -> Set<string> -> Result<byte[] * ContentType, SerializationError>
type Deserializer = byte[] -> ContentType -> Type -> Map<string, obj> -> Result<obj, SerializationError>

