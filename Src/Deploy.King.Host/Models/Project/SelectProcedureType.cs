using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;
using Deploy.Procedures;
using Deploy.Procedures.Examples;

namespace Deploy.King.Host.Models.Project
{
    public class SelectProcedureType
    {
        readonly List<Type> procedureTypes;

        public SelectProcedureType()
        {
            procedureTypes = typeof(Energy10WithoutMigrations).Assembly.GetTypes().Where(x => !x.IsAbstract && x.IsClass && typeof(IProcedure).IsAssignableFrom(x)).ToList();
        }

        [DisplayName("Procedure")]
        public string ProcedureType { get; set; }

        public SelectList ProcedureTypes
        {
            get { return new SelectList(procedureTypes, "Fullname", "Name", procedureTypes[0]); }
        }
    }
}