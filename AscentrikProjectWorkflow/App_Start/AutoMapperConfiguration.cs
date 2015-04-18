using AscentrikProjectWorkflow.DataModel;
using AscentrikProjectWorkflow.ViewModel;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AscentrikProjectWorkflow.App_Start
{
    public class AutoMapperConfiguration
    {
        public static void Configure()
        {
            Mapper.Initialize(x =>
            {
                Mapper.CreateMap<Client, ClientViewModel>();
                Mapper.CreateMap<List, ListViewModel>();
                Mapper.CreateMap<ListType, ListTypeViewModel>();
                Mapper.CreateMap<User, UserViewModel>();
                Mapper.CreateMap<Project, ProjectViewModel>();
                Mapper.CreateMap<ProjectCosting, ProjectCostingViewModel>();
            });
        }
    }
}