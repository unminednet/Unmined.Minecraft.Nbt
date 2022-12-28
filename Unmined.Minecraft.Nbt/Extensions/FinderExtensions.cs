using System.Diagnostics.CodeAnalysis;
using Unmined.Minecraft.Nbt.Tags;

namespace Unmined.Minecraft.Nbt.Extensions;

public static class FinderExtensions
{
    public static bool TryFind<T>(this CompoundTag root, string path, [MaybeNullWhen(false)] out T tag) where T : Tag
    {
        tag = root.Find<T>(path);
        return tag != null;
    }

    public static bool TryFind(this CompoundTag root, string path, [MaybeNullWhen(false)] out Tag tag)
    {
        tag = root.Find(path);
        return tag != null;
    }

    public static T? Find<T>(this CompoundTag root, string path) where T : Tag
    {
        var result = root.Find(path);
        if (result is T tagFound)
            return tagFound;

        return null;
    }

    public static Tag? Find(this CompoundTag root, string path)
    {
        while (true)
        {
            var i = path.IndexOf('/');
            var name = i < 0 ? path : path[..i];
            if (!root.TryGetValue(name, out var tag))
                return null;

            if (i < 0)
                return tag;

            if (tag is not CompoundTag c)
                return null;

            root = c;
            path = path[(i + 1)..];
        }
    }
}