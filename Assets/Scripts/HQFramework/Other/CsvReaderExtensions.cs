using System.Collections;
using System.Collections.Generic;

using LumenWorks.Framework.IO.Csv;
using UnityEngine;

namespace HQFrameWork
{
    public static class CsvReaderExtensions
    {

        public static T ToData<T>(this CsvReader csvReader) where T : BaseData,new()
        {
            var Ins = BaseData.GetData<T>();
            string[] temp = new string[100];
            csvReader.CopyCurrentRecordTo(temp);
            return Ins;

        }
    }

}

