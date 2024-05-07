using MessagePack;
using MessagePack.Formatters;
using System;
using UnityEngine;

public class Vector2Formatter : IMessagePackFormatter<Vector2>
{
    public void Serialize(ref MessagePackWriter writer, Vector2 value, MessagePackSerializerOptions options)
    {
        writer.WriteArrayHeader(2);
        writer.Write((int)(value.x * 100));
        writer.Write((int)(value.y * 100));
    }

    public Vector2 Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadArrayHeader(out var count) && count == 2)
        {
            var x = reader.ReadInt32() / 100f;
            var y = reader.ReadInt32() / 100f;
            return new Vector2(x, y);
        }
        throw new InvalidOperationException("Unexpected length or format for Vector2");
    }
}
