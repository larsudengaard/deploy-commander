using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;
using Deploy.King.Procedures;
using Deploy.King.Procedures.Energy10;

namespace Deploy.Models.Project
{
    public class AddArguments
    {
        
    }
    public class SelectProcedureType
    {
        readonly List<Type> procedureTypes;

        public SelectProcedureType()
        {
            procedureTypes = typeof(DeployWithoutMigrations).Assembly.GetTypes().Where(x => !x.IsAbstract && x.IsClass && typeof(IProcedure).IsAssignableFrom(x)).ToList();
        }

        [DisplayName("Procedure")]
        public string ProcedureType { get; set; }

        public SelectList ProcedureTypes
        {
            get { return new SelectList(procedureTypes, "Fullname", "Name", procedureTypes[0]); }
        }
    }
}