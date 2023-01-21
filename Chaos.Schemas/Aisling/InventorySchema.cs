using System.Collections;
using System.Text.Json.Serialization;
using Chaos.Common.Converters;

namespace Chaos.Schemas.Aisling;

[JsonConverter(typeof(EnumerableConverter<InventorySchema, ItemSchema>))]
public sealed class InventorySchema : IEnumerable<ItemSchema>
{
    public ICollection<ItemSchema> Items { get; set; }

    public InventorySchema(IEnumerable<ItemSchema> items) => Items = items.ToList();

    public IEnumerator<ItemSchema> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}