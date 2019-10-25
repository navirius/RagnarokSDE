using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Database;
using Newtonsoft.Json.Converters;
using SDE.Editor.Generic;
using SDE.Editor.Generic.Lists;

namespace SDE.Editor
{
    public static class Utils
    {
        public static string FindAttributeValueById(MetaTable<int> dataTable, string idValue, DbAttribute dbAttribute)
        {

            foreach (var tupleItem in dataTable.FastItems)
            {
                if (tupleItem.Key.ToString() == idValue)
                {
                    return (string) (object) tupleItem.GetValue(dbAttribute.Index);
                    break;
                }
            }

            return default(string);
        }
        public static TKey FindMobIDBySpriteName<TKey>(MetaTable<int> mobTable, string par_spriteName)
        {
            int mobId = 0;
            foreach (var tupleItem in mobTable.FastItems)
            {

                string spriteName = tupleItem.GetStringValue(ServerMobAttributes.SpriteName.Index);
                if (spriteName.ToLower() == par_spriteName.ToLower())
                {
                    mobId = tupleItem.Key;
                    break;
                }
            }

            return (TKey)(object)mobId;
        }


        public static bool IsMobExistBySpriteName<TKey>(MetaTable<int> mobTable, string par_spriteName)
        {
            TKey mobId = FindMobIDBySpriteName<TKey>(mobTable, par_spriteName);
            return (int)(object) mobId > 0;
        }

        public static bool IsMobExistBySpriteName<TKey>(MetaTable<int> mobTable, string par_spriteName, out TKey mobId)
        {
            mobId = FindMobIDBySpriteName<TKey>(mobTable, par_spriteName);
            return (int)(object)mobId > 0;
        }

        public static TKey FindItemIdByAegisName<TKey>(MetaTable<int> itemTable, string parAegisName)
        {
            int itemId = 0;
            foreach (var tupleItem in itemTable.FastItems)
            {

                string aegisName = tupleItem.GetStringValue(ServerItemAttributes.AegisName.Index);
                if (aegisName.ToLower() == parAegisName.ToLower())
                {
                    itemId = tupleItem.Key;
                    break;
                }
            }

            return (TKey)(object)itemId;
        }

        public static bool IsItemExistByAegisName<TKey>(MetaTable<int> itemTable, string parAegisName, out TKey itemId)
        {
            itemId = FindItemIdByAegisName<TKey>(itemTable, parAegisName);
            return (int)(object)itemId > 0;
        }
    }
}
