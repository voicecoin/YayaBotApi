﻿using Core.Menu;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IInitializationLoader
    {
        int Priority { get; }
        void Initialize(IHostingEnvironment env);
    }

    /// <summary>
    /// All table should implement this table inteface
    /// </summary>
    public interface IDbRecord
    {
        bool IsExist(CoreDbContext dc);
    }
    public interface IDbRecord4SqlServer { }

    /// <summary>
    /// Domain model, based on Domain-Driven Design concept.
    /// Business model shold implement this interface.
    /// </summary>
    public interface IDomainModel
    {
    }

    public interface IDomainModel<T> where T : IDbRecord
    {
        Boolean AddEntity();
        Boolean RemoveEntity();
        T LoadEntity();
    }

    /// <summary>
    /// Initialize data for modules
    /// </summary>
    public interface IHookDbInitializer
    {
        /// <summary>
        /// value smaller is higher priority
        /// </summary>
        int Priority { get; }
        void Load(IHostingEnvironment env, CoreDbContext dc);
    }

    public interface IDbTableAmend
    {
        void Amend(CoreDbContext dc);
    }

    public interface IHookMenu
    {
        void UpdateMenu(List<VmMenu> menus, CoreDbContext dc);
    }
}
