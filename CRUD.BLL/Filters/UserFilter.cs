using CRUD.BLL.Enums;
using CRUD.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRUD.BLL.Filters
{
    public class UserFilter : FilterBase<User>
    {
        public string? UserName { get; set; }
        public string? Name { get; set; }
        public long? Id { get; set; }
        public int? UserTypeId { get; set; }
        public string? OrderBy { get; set; }
        public override IQueryable<User> CreateQuery(IQueryable<User> query)
        {
            if (Query != null)
                return Query;

            if (!string.IsNullOrEmpty(UserName))
            {
                query = query.Where(x => x.UserName.ToLower().Contains(UserName.ToLower()));
            }

            if (Id.HasValue)
            {
                query = query.Where(x => x.Id == Id);
            }

            if (!string.IsNullOrEmpty(Name))
            {
                query = query.Where(x => x.Name.ToLower().Contains(Name.ToLower()));
            }

            if (UserTypeId.HasValue)
            {
                query = query.Where(x => x.UserTypeId == UserTypeId);
            }

            if (OrderBy != null)
            {
                switch (OrderBy)
                {
                    case nameof(OrderByEnum.Id):
                        return query.OrderBy(x => x.Id);
                    case nameof(OrderByEnum.IdDesc):
                        return query.OrderByDescending(x => x.Id);
                    case nameof(OrderByEnum.Name):
                        return query.OrderBy(x => x.Name);
                    case nameof(OrderByEnum.NameDesc):
                        return query.OrderByDescending(x => x.Name);
                    case nameof(OrderByEnum.UserName):
                        return query.OrderBy(x => x.UserName);
                    case nameof(OrderByEnum.UserNameDesc):
                        return query.OrderByDescending(x => x.UserName);
                    case nameof(OrderByEnum.Type):
                        return query.OrderBy(x => x.UserTypeId);
                    case nameof(OrderByEnum.TypeDesc):
                        return query.OrderByDescending(x => x.UserTypeId);
                }
            }

            return query.OrderByDescending(x => x.CreatedDate);
        }

        public override IQueryable<User> FilterObjects(IQueryable<User> query)
        {
            query = CreateQuery(query);
            return base.FilterObjects(query);
        }
    }
}
