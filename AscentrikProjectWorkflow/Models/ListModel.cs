using AscentrikProjectWorkflow.DataModel;
using AscentrikProjectWorkflow.ViewModel;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace AscentrikProjectWorkflow.Models
{
    public class ListModel
    {
        private AscentrikProjectWorkflowEntities db = new AscentrikProjectWorkflowEntities();

        #region LIST TYPE

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal List<ListTypeViewModel> GetListTypes()
        {
            var tblLists = db.ListTypes.OrderBy(x => x.Type).ToList();
            var model = Mapper.Map<List<ListType>, List<ListTypeViewModel>>(tblLists);
            return model;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal List<ListTypeViewModel> GetActiveListTypes()
        {
            var tblLists = db.ListTypes.Where(x => x.IsActive).OrderBy(x => x.Type).ToList();
            var model = Mapper.Map<List<ListType>, List<ListTypeViewModel>>(tblLists);
            return model;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        internal int AddListType(ListTypeViewModel model)
        {
            if (model != null)
            {
                var tblLists = db.ListTypes.Count(x => x.Type == model.Type);
                if (tblLists > 0)
                    return 2; // List is already present in the database.
                var list = new ListType
                {
                    Type = model.Type,
                    IsActive = model.IsActive,
                    CreatedBy = model.CreatedBy,
                    CreatedOn = model.CreatedOn
                };

                db.ListTypes.Add(list);
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
        internal ListTypeViewModel GetListTypeById(int id)
        {
            var tblLists = db.ListTypes.FirstOrDefault(x => x.Id == id);
            var listModel = Mapper.Map<ListType, ListTypeViewModel>(tblLists);
            return listModel;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        internal int EditListType(ListTypeViewModel model)
        {
            if (model != null)
            {
                var tblListTypes = db.ListTypes.Count(x => x.Type == model.Type);
                if (tblListTypes > 1)
                    return 2; // List is already present in the database.

                var list = db.ListTypes.Where(x => x.Id == model.Id).FirstOrDefault();
                if (list == null)
                    return 3; //Data is not available in the database.

                var lists = db.Lists.Where(x => x.Type == model.Id && x.IsActive);

                if (lists.Count() > 0 && !model.IsActive)
                {
                    return 4;
                }

                list.Type = model.Type;
                list.IsActive = model.IsActive;
                list.EditedBy = model.EditedBy;
                list.EditedOn = model.EditedOn;

                db.Entry(list).State = EntityState.Modified;
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
        internal int DeleteListType(int id)
        {
            if (id > 0)
            {
                var item = db.ListTypes.FirstOrDefault(x => x.Id == id);
                if (item == null)
                    return 1;//Invalid client id.

                var projects = db.Lists.Count(x => x.Type == id);
                if (projects > 0)
                    return 2; //Active List is associated with lists.

                db.ListTypes.Remove(item);
                db.SaveChanges();

                return 0;
            }
            return 1; // Invalid id
        }

        #endregion

        #region LIST

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal List<ListViewModel> GetLists()
        {
            var tblLists = db.Lists.Include("Client").Include("ListType").OrderByDescending(x => x.CreatedOn).ThenByDescending(x => x.EditedOn).ToList();
            var model = Mapper.Map<List<List>, List<ListViewModel>>(tblLists);
            return model;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        internal int AddList(ListViewModel model)
        {
            if (model != null)
            {
                var tblLists = db.Lists.Count(x => x.Code == model.Code);
                if (tblLists > 0)
                    return 2; // List is already present in the database.
                var list = new List
                {
                    Code = model.Code,
                    Type = model.Type,
                    Reference = model.Reference,
                    IsActive = model.IsActive,
                    ClientId = model.ClientId,
                    ListType = model.ListType,
                    CreatedBy = model.CreatedBy,
                    CreatedOn = model.CreatedOn
                };

                db.Lists.Add(list);
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
        internal ListViewModel GetListById(int id)
        {
            var tblLists = db.Lists.FirstOrDefault(x => x.Id == id);
            var listModel = Mapper.Map<List, ListViewModel>(tblLists);
            return listModel;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        internal int EditList(ListViewModel model)
        {
            if (model != null)
            {
                var tblLists = db.Lists.Count(x => x.Code == model.Code);
                if (tblLists > 1)
                    return 2; // List is already present in the database.

                var list = db.Lists.Where(x => x.Id == model.Id).FirstOrDefault();
                if (list == null)
                    return 3; //Data is not available in the database.

                if (list.ClientId != model.ClientId || !model.IsActive)
                {
                    var projects = db.Projects.Where(x => x.ListId == model.Id && x.IsActive);
                    if (projects.Any())
                    {
                        if (list.ClientId != model.ClientId)
                            return 4; //cannot change the client as few projects are associated with this list
                        else
                            return 5; //Cannot deactivate list as few projects are associated with this list 
                    }
                }

                list.Code = model.Code;
                list.Type = model.Type;
                list.Reference = model.Reference;
                list.IsActive = model.IsActive;
                list.ClientId = model.ClientId;
                list.ListType = model.ListType;
                list.EditedBy = model.EditedBy;
                list.EditedOn = model.EditedOn;

                db.Entry(list).State = EntityState.Modified;
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
        internal int DeleteList(int id)
        {
            if (id > 0)
            {
                var item = db.Lists.FirstOrDefault(x => x.Id == id);
                if (item == null)
                    return 1;//Invalid client id.

                var projects = db.Projects.Count(x => x.ListId == id);
                if (projects > 0)
                    return 2; //Active Project is associated with lists.

                db.Lists.Remove(item);
                db.SaveChanges();

                return 0;
            }
            return 1; // Invalid id
        }

        #endregion
    }
}