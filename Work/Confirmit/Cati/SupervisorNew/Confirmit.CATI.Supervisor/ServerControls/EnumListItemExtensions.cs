using Confirmit.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

public static class EnumListItemExtensions
{
    public static void FillWithEnumOptions<TEnum>(
        this List<ListItem> items,
        bool clearFirst = true,
        params (TEnum Value, bool Permitted)[] options)
        where TEnum : struct, Enum
    {
        if (items == null) throw new ArgumentNullException(nameof(items));
        if (options == null) throw new ArgumentNullException(nameof(options));

        if (clearFirst)
            items.Clear();

        // Build a lookup for explicit permissions (deny/allow list entries),
        // where later entries override earlier ones.
        var permissionMap = new Dictionary<TEnum, bool>();
        foreach (var (value, permitted) in options)
            permissionMap[value] = permitted;

        // Enumerate all enum values; exclude those explicitly marked as not permitted.
        IEnumerable<TEnum> allValues =
#if NET5_0_OR_GREATER
            Enum.GetValues<TEnum>();
#else
            Enum.GetValues(typeof(TEnum)).Cast<TEnum>();
#endif

        var listItems = allValues
            .Where(v => !permissionMap.TryGetValue(v, out var permitted) || permitted)
            .Select(v => new ListItem(
                v.ToString(),
                // numeric string for the enum value; safe for any underlying type
                Enum.Format(typeof(TEnum), v, "D")
            ));

        items.AddRange(listItems);
    }

    public static void FillWithEnumOptions<TEnum>(
        this List<ListItem> items,
        params (TEnum Value, bool Permitted)[] options)
        where TEnum : struct, Enum
    {
        items.FillWithEnumOptions(clearFirst: true, options);
    }

    public static void FillWithEnumOptions<TEnum>(
        this ListItemCollection items,
        params (TEnum Value, bool Permitted)[] options)
        where TEnum : struct, Enum
    {
        var listItems = new List<ListItem>();
        listItems.FillWithEnumOptions(clearFirst: true, options);
        listItems.ForEach(item => {
            items.Add(item);
        });
    }
}
