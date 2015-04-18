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
    public class ClientModel
    {
        private AscentrikProjectWorkflowEntities db = new AscentrikProjectWorkflowEntities();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal List<ClientViewModel> GetClientList()
        {
            var tblClients = db.Clients.OrderByDescending(x => x.CreatedOn).ThenByDescending(x => x.EditedOn).ToList();
            var clientModel = Mapper.Map<List<Client>, List<ClientViewModel>>(tblClients);
            return clientModel;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal List<ClientViewModel> GetActiveClientList()
        {
            var tblClients = db.Clients.Where(x => x.IsActive).OrderBy(x => x.Name).ToList();
            var clientModel = Mapper.Map<List<Client>, List<ClientViewModel>>(tblClients);
            return clientModel;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        internal int AddClient(ClientViewModel model)
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
        internal ClientViewModel GetClientById(int id)
        {
            var tblClients = db.Clients.FirstOrDefault(x => x.Id == id);
            var clientModel = Mapper.Map<Client, ClientViewModel>(tblClients);
            return clientModel;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        internal int EditClient(ClientViewModel model)
        {
            if (model != null)
            {
                var tblClients = db.Clients.Count(x => x.Code == model.Code);
                if (tblClients > 1)
                    return 2; // Client is already present in the database.

                var client = db.Clients.Where(x => x.Id == model.Id).FirstOrDefault();
                if (client == null)
                    return 3; //Data is not available in the database.

                client.Code = model.Code;
                client.Name = model.Name;
                client.IsActive = model.IsActive;
                client.EditedBy = model.EditedBy;
                client.EditedOn = model.EditedOn;

                db.Entry(client).State = EntityState.Modified;
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
        internal int DeleteClient(int id)
        {
            if (id > 0)
            {
                var item = db.Clients.FirstOrDefault(x => x.Id == id);
                if (item == null)
                    return 1;//Invalid client id.

                var projects = db.Lists.Count(x => x.ClientId == id);
                if (projects > 0)
                    return 2; //Clients is associated with lists.

                db.Clients.Remove(item);
                db.SaveChanges();

                return 0;
            }
            return 1; // Invalid id

        }
    }
}