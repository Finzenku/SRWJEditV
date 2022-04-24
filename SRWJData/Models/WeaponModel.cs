using SRWJData.Attributes;
using SRWJData.Extensions;
using SRWJData.Utilities;

namespace SRWJData.Models
{
    [GameObject(initialAddress: 0xAB268, dataLength: 0x20, objectCount: 1469, pointerOffsets: new int[] { 0, 4 })]
    public class WeaponModel : INameable, IDataObject
    {
        public int Index { get; set; }
        public int NamePointer1 { get; set; }
        public int NamePointer2 { get; set; }
        public byte Flag { get; set; }
        public byte RequiredSkill { get; set; }
        public byte UseConditions { get; set; }
        public byte Characteristic { get; set; }
        public short Attack { get; set; }
        public byte Energy { get; set; }
        public sbyte Hit { get; set; }
        public sbyte Critical { get; set; }
        public byte Bullet { get; set; }
        public byte Spirit { get; set; }
        public byte BGM { get; set; }
        public sbyte MaximumRange { get; set; }
        public sbyte MinimumRange { get; set; }
        public byte UnitGroupCombo { get; set; }
        public byte PilotGroupCombo { get; set; }
        public byte AddedEffect { get; set; }
        public byte BulletFlag { get; set; }
        public byte AttackUpgrade { get; set; }
        public byte AdaptationSky { get; set; }
        public byte AdaptationLand { get; set; }
        public byte AdaptationSea { get; set; }
        public byte AdaptationCosmos { get; set; }
        public byte Unknown { get; set; }

        public WeaponModel()
        {
            AddedEffect = 6;
            BGM = 6;
        }
        public WeaponModel(byte[] weaponData)
        {
            NamePointer1 = weaponData.GetInt(0);
            NamePointer2 = weaponData.GetInt(4);
            Flag = weaponData[8];
            RequiredSkill = weaponData[9];
            UseConditions = weaponData[10];
            Characteristic = weaponData[11];
            Attack = weaponData.GetShort(12);
            Energy = weaponData[14];
            Hit = (sbyte)weaponData[15];
            Critical = (sbyte)weaponData[16];
            Bullet = weaponData[17];
            Spirit = weaponData[18];
            BGM = weaponData[19];
            MaximumRange = (sbyte)weaponData[20];
            MinimumRange = (sbyte)weaponData[21];
            UnitGroupCombo = weaponData[22];
            PilotGroupCombo = weaponData[23];
            AddedEffect = weaponData[24];
            BulletFlag = weaponData[25];
            AttackUpgrade = weaponData[26];
            AdaptationSky = weaponData[27];
            AdaptationLand = weaponData[28];
            AdaptationSea = weaponData[29];
            AdaptationCosmos = weaponData[30];
            Unknown = weaponData[31];
        }

        public byte[] GetData()
        {
            byte[] p1 = NamePointer1.GetBytes(),
                p2 = NamePointer2.GetBytes(),
                atk = Attack.GetBytes();
            return new byte[32]
            {
                p1[0], p1[1], p1[2], p1[3],
                p2[0], p2[1], p2[2], p2[3],
                Flag, RequiredSkill, UseConditions, Characteristic,
                atk[0], atk[1], Energy, (byte)Hit,
                (byte)Critical, Bullet, Spirit, BGM,
                (byte)MaximumRange, (byte)MinimumRange, UnitGroupCombo, PilotGroupCombo,
                AddedEffect, BulletFlag, AttackUpgrade, AdaptationSky,
                AdaptationLand, AdaptationSea, AdaptationCosmos, Unknown,
            };
        }

        public int[] GetNameAddresses() => new int[] { NamePointer1, NamePointer2 };
    }
}
