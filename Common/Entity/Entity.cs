namespace Common.Entity;

public abstract class Entity
{
    public virtual long Id { get; protected internal set; }
    public virtual DateTime CreatedOn { get; protected internal set; }
    public virtual DateTime? ModifiedOn { get; protected internal set; }
    public override bool Equals(object obj)
    {
        var other = obj as Entity;

        if (ReferenceEquals(other, null))
            return false;

        if (ReferenceEquals(this, other))
            return true;

        if (this.GetType() != other.GetType()) // limitation in lazy loading this entity will wrap with proxy class
            return false;

        if (Id == 0 || other.Id == 0)
            return false;

        return Id == other.Id;
    }

    public static bool operator ==(Entity a, Entity b)
    {
        if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
            return true;

        if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
            return false;

        return a.Equals(b);
    }

    public static bool operator !=(Entity a, Entity b)
    {
        return !(a == b);
    }

    public override int GetHashCode()
    {
        return (this.ToString() + Id).GetHashCode();
    }

}