using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dziennik.ViewModels
{

    public class FileModel
    {
        public string FileName { get; set; }
        public string FileSizeText { get; set; }
        public DateTime FileAccessed { get; set; }
    }

    public class ExplorerModel
    {
        public List<FileModel> fileModelList;

        public ExplorerModel(List<FileModel> _fileModelList)
        {
            fileModelList = _fileModelList;
        }
    }
}