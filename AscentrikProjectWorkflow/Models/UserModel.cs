using AscentrikProjectWorkflow.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AscentrikProjectWorkflow.ViewModel;
using AutoMapper;
using System.Data;

namespace AscentrikProjectWorkflow.Models
{
    public class UserModel
    {
        private AscentrikProjectWorkflowEntities db = new AscentrikProjectWorkflowEntities();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal List<UserViewModel> GetUserList()
        {
            var tblClients = db.Users.Include("Role1").OrderBy(x => x.EmailAddress).ToList();
            var model = Mapper.Map<List<User>, List<UserViewModel>>(tblClients);
            return model;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal Dictionary<int, string> GetRoleList()
        {
            var tblRoles = db.Roles.OrderByDescending(x => x.Name).ToList();
            var model = tblRoles.ToDictionary(x => x.Id, x => x.Name);
            return model;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        internal int AddUser(ClientViewModel model)
        {
            if (model != null)
            {
                var tblClients = db.Clients.Count(x => x.Code == model.Code);
                if (tblClients > 0)
                    return 2; // Client is already present in the database.
                var client = new Client
                {
                    Code = model.Code,
                    Name = model.Name,
                    IsActive = model.IsActive,
                    CreatedBy = model.CreatedBy,
                    CreatedOn = model.CreatedOn
                };

                db.Clients.Add(client);
                db.SaveChanges();
                return 0;
            }
            return 1; //Null object
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        internal UserViewModel GetUserById(int id)
        {
            var tblUser = db.Users.FirstOrDefault(x => x.Id == id);
            var model = Mapper.Map<User, UserViewModel>(tblUser);
            return model;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        internal int EditUser(UserViewModel model)
        {
            if (model != null)
            {
                var tblUsers = db.Users.Count(x => x.EmailAddress == model.EmailAddress);
                if (tblUsers > 1)
                    return 2; // User is already present in the database.

                var user = db.Users.Where(x => x.Id == model.Id).FirstOrDefault();
                if (user == null)
                    return 3; //Data is not available in the database.

                user.EmailAddress = model.EmailAddress;
                user.DisplayName = model.EmailAddress.Split('@')[0];
                user.Role = model.Role;
                user.IsActive = model.IsActive;
                user.UpdatedBy = model.UpdatedBy;
                user.LastUpdatedDate = model.LastUpdatedDate;

                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return 0;
            }
            return 1; //Null object
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        internal int DeleteUser(int id)
        {
            if (id > 0)
            {
                var item = db.Users.FirstOrDefault(x => x.Id == id);
                if (item == null)
                    return 1;//Invalid client id.

                //var projects = db.AllotedToes.Count(x => x.Id == id);
                //if (projects > 0)
                //    return 2; //Clients is associated with lists.

                db.Users.Remove(item);
                db.SaveChanges();

                return 0;
            }
            return 1; // Invalid id

        }

    }
}