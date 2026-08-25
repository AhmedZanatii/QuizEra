public class BaseEntity
{
    public string CreatorUser { get; private set; }
    public DateTime CreatedDate { get; private set; }
    public string? ModifierUser { get; private set; }
    public DateTime? ModifiedDate { get; private set; }
    public bool IsDeleted { get; private set; } = false;
    public string? DeleterUser { get; private set; }
    public DateTime? DeletedDate { get; private set; }

    protected BaseEntity() { }

    public BaseEntity(string creatorUser)
    {
        CreatorUser = creatorUser;
        CreatedDate = DateTime.UtcNow;
    }

    public void Update(string modifierUser)
    {
        ModifierUser = modifierUser;
        ModifiedDate = DateTime.UtcNow;
    }

    public bool Delete(string deleterUser, DateTime deletedDate)
    {
        if (IsDeleted)
            return false;

        IsDeleted = true;
        DeleterUser = deleterUser;
        DeletedDate = deletedDate;

        return true;
    }

    public bool Restore()
    {
        if (!IsDeleted)
            return false;

        IsDeleted = false;
        DeleterUser = null;
        DeletedDate = null;
        return true;
    }
}