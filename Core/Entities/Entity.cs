using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities;

public class Entity<TId>
{
    public TId Id { get; set; }
    public Entity()
    {
        Id = default;
    }
    public Entity(TId id)
    {
        Id = id;
    }
}
