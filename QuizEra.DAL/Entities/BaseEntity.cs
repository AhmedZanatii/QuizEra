using System;
using System.Collections.Generic;
using System.Text;

namespace QuizEra.DAL.Entities
{
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
        
        // Return true if deleted successfully, false if already deleted
        public bool Delete(string deleterUser, DateTime deletedDate)
        {
            if(IsDeleted) return false;
            IsDeleted = true;
            DeleterUser = deleterUser;
            DeletedDate = deletedDate;
            return true;
        }
    }
}
