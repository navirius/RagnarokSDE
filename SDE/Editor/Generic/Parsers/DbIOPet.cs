using System.IO;
using System.Text;
using SDE.Editor.Generic.Core;
using SDE.Editor.Generic.Lists;
using SDE.Editor.Generic.Parsers.Generic;
using SDE.Editor.Generic.TabsMakerCore;
using SDE.Editor.Generic.YamlModel;
using SDE.View;
using YamlDotNet.Serialization;

namespace SDE.Editor.Generic.Parsers
{
    public sealed class DbIOPet
    {
        public static void Loader<TKey>(DbDebugItem<TKey> debug, AbstractDb<TKey> db)
        {
            if (debug.FileType == FileType.Txt)
            {
                DbIOMethods.DbWriterComma((DbDebugItem<int>)(object)debug, (AbstractDb<int>)(object)db);
            }
            else if (debug.FileType == FileType.Yaml)
            {
                var table = debug.AbsractDb.Table;
                var input = new StreamReader(debug.FilePath, Encoding.UTF8);
                var deserializer = new DeserializerBuilder().Build();

                var pet = deserializer.Deserialize<PetDbModel>(input);

                if(pet!=null)
                {
                    foreach(var bodyItem in pet.Body)
                    {
                        var mobTable = SdeEditor.Instance.ProjectDatabase.GetMetaTable<int>(ServerDbs.Mobs);
                        TKey mobId = FindMobID<TKey>(mobTable, bodyItem);
                        var itemTable = SdeEditor.Instance.ProjectDatabase.GetMetaTable<int>(ServerDbs.Items);
                        int mobIntId = (int) (object) (mobId);
                        if (mobIntId != 0)
                        {
                            table.SetRaw(mobId, ServerPetAttributes.Name, bodyItem.Mob);
                            table.SetRaw(mobId, ServerPetAttributes.JName, bodyItem.Mob);
                            table.SetRaw(mobId, ServerPetAttributes.TameItemId, bodyItem.TameItem);
                            table.SetRaw(mobId, ServerPetAttributes.EggId, bodyItem.EggItem);
                            table.SetRaw(mobId, ServerPetAttributes.EquipId, bodyItem.EquipItem);
                            table.SetRaw(mobId, ServerPetAttributes.EquipId, bodyItem.EquipItem);
                            table.SetRaw(mobId, ServerPetAttributes.FoodId, bodyItem.FoodItem);
                            table.SetRaw(mobId, ServerPetAttributes.Fullness, bodyItem.Fullness);
                            table.SetRaw(mobId, ServerPetAttributes.HungryDelay, bodyItem.HungryDelay);
                            table.SetRaw(mobId, ServerPetAttributes.HungerIncrease, bodyItem.HungerIncrease);
                            table.SetRaw(mobId, ServerPetAttributes.Intimate, bodyItem.IntimacyStart);
                            table.SetRaw(mobId, ServerPetAttributes.RHungry, bodyItem.IntimacyFed);
                            table.SetRaw(mobId, ServerPetAttributes.RFull, bodyItem.IntimacyOverfed);
                            table.SetRaw(mobId, ServerPetAttributes.IntimacyHungry, bodyItem.IntimacyHungry);
                            table.SetRaw(mobId, ServerPetAttributes.Die, bodyItem.IntimacyOwnerDie);
                            table.SetRaw(mobId, ServerPetAttributes.Capture, bodyItem.CaptureRate);
                            table.SetRaw(mobId, ServerPetAttributes.SPerformance, bodyItem.SpecialPerformance);
                            table.SetRaw(mobId, ServerPetAttributes.AttackRate, bodyItem.AttackRate);
                            table.SetRaw(mobId, ServerPetAttributes.Retaliate, bodyItem.RetaliateRate);
                            table.SetRaw(mobId, ServerPetAttributes.ChangeTargetRate, bodyItem.ChangeTargetRate);
                            table.SetRaw(mobId, ServerPetAttributes.PetScript, bodyItem.Script);
                            table.SetRaw(mobId, ServerPetAttributes.LoyalScript, bodyItem.SupportScript);
                            foreach (PetEvolutionItemModel itemModel in bodyItem.Evolution)
                            {
                                
                            }
                            //table.SetRaw(mobId, ServerPetAttributes.EggId);
                        }
                        
                    }
                }
            }
        }

        private static int FindItemID(MetaTable<int> itemTable, string itemName)
        {
            int itemId = 0;
            foreach (var tupleItem in itemTable.Tuples)
            {
                if (tupleItem.Value.GetValue<string>(ServerItemAttributes.AegisName) ==itemName)
                {
                    itemId = tupleItem.Key;
                    break;
                }
            }

            return itemId;
        }
        private static TKey FindMobID<TKey>(MetaTable<int> mobTable, PetItemBodyModel bodyItem)
        {
            int mobId = 0;
            foreach (var tupleItem in mobTable.FastItems)
            {

                string spriteName = tupleItem.GetStringValue(ServerMobAttributes.SpriteName.Index);
                if (spriteName.ToLower() == bodyItem.Mob.ToLower())
                {
                    mobId = tupleItem.Key;
                    break;
                }
            }

            return (TKey)(object)mobId;
        }

        public static void WriteEntry<TKey>(StringBuilder builder, ReadableTuple<TKey> tuple)
        {
            
        }

        public static void Writer<TKey>(DbDebugItem<TKey> debug, AbstractDb<TKey> db)
        {
            
        }
    }
}