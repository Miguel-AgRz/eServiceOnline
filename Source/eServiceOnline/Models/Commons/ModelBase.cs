//using Sanjel.Common.BusinessEntities.Lookup;

using System.Collections.Generic;
using MetaShare.Common.Foundation.EntityBases;
using Microsoft.Extensions.Caching.Memory;
using Sesi.SanjelData.Entities.BusinessEntities.Operation.Dispatch;
using Sesi.SanjelData.Entities.Common.BusinessEntities.WellSite;

namespace eServiceOnline.Models.Commons
{
    public class ModelBase<TEntity>
    {
        public const string MenuUpdateNotes = "Update Notes";

        public string LoggedUser { get; set; }

        public NoteModel NoteModel { get; set; }

        public IMemoryCache _memoryCache { get; set; }


        public virtual void PopulateFrom(TEntity entity)
        {
        }

        public virtual void PopulateTo(TEntity entity)
        {
        }

        protected string GetDownRigSuffix(RigJob rigJob)
        {
            string suffix = null;

//            if (rigJob.JobLifeStatus == JobLifeStatus.None)
            if (rigJob.JobLifeStatus==(JobLifeStatus.None))
                suffix = rigJob.RigStatus==(RigStatus.Active) ? null : "down";
            return suffix;
        }

        protected List<ContextMenu> SetNoteContextMenus(string entityId)
        {
            List<ContextMenu> list = new List<ContextMenu>();

            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(NoteModel.Notes);
            string note =  System.Convert.ToBase64String(plainTextBytes);

            list.Add(new ContextMenu
            {
                MenuName = MenuUpdateNotes,
                ProcessingMode = ProcessingMode.PopsUpWindow,
                //Nov 30, 2023 zhangyuan 203_PR_UpdateBinNotes: modify Add PodIndex
                Parms = new List<string>() { entityId, NoteModel.PodIndex.ToString(), NoteModel.ReturnActionName, NoteModel.ReturnControllerName, NoteModel.PostControllerName, NoteModel.PostMethodName, note },
                ControllerName = NoteModel.CallingControllerName,
                ActionName = NoteModel.CallingMethodName
            });

            return list;
        }

    }
}