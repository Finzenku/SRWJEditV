using SRWJEditV.Attributes;
using SRWJEditV.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRWJEditV.Models
{
    [GameObject(initialAddress: 0xAB268, dataLength: 0x20, objectCount: 1469, pointerOffsets: new int[] { 0, 4 })]
    public class Weapon : INameable, IDataObject
    {
        public int Index { get; set; }
        public int NamePointer1 { get; set; }
        public int NamePointer2 { get; set; }
        public byte Flag { get; set; }
        public byte NeededSkill { get; set; }
        public byte EnabledTerm { get; set; }
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
        public byte UnitGroupOfUnion { get; set; }
        public byte PilotGroupOfUnion { get; set; }
        public byte SpecialEffecive { get; set; }
        public byte BulletFlag { get; set; }
        public byte RaisedPoint { get; set; }
        public byte AdaptationSky { get; set; }
        public byte AdaptationLand { get; set; }
        public byte AdaptationSea { get; set; }
        public byte AdaptationCosmos { get; set; }
        public byte Unknown { get; set; }

        public Weapon()
        {
        }
        public Weapon(byte[] weaponData)
        {
            NamePointer1 = LittleEndian.GetInt32(weaponData.Take<byte>(4).ToArray());
            NamePointer2 = LittleEndian.GetInt32(weaponData.Take<byte>(new Range(4, 8)).ToArray());
            Flag = weaponData[8];
            NeededSkill = weaponData[9];
            EnabledTerm = weaponData[10];
            Characteristic = weaponData[11];
            Attack = LittleEndian.GetInt16(new byte[] { weaponData[12], weaponData[13] });
            Energy = weaponData[14];
            Hit = (sbyte)weaponData[15];
            Critical = (sbyte)weaponData[16];
            Bullet = weaponData[17];
            Spirit = weaponData[18];
            BGM = weaponData[19];
            MaximumRange = (sbyte)weaponData[20];
            MinimumRange = (sbyte)weaponData[21];
            UnitGroupOfUnion = weaponData[22];
            PilotGroupOfUnion = weaponData[23];
            SpecialEffecive = weaponData[24];
            BulletFlag = weaponData[25];
            RaisedPoint = weaponData[26];
            AdaptationSky = weaponData[27];
            AdaptationLand = weaponData[28];
            AdaptationSea = weaponData[29];
            AdaptationCosmos = weaponData[30];
            Unknown = weaponData[31];
        }

        public byte[] GetData()
        {
            byte[] p1 = LittleEndian.GetBytes(NamePointer1), p2 = LittleEndian.GetBytes(NamePointer2), atk = LittleEndian.GetBytes(Attack);
            return new byte[32]
            {
                p1[0], p1[1], p1[2], p1[3],
                p2[0], p2[1], p2[2], p2[3],
                Flag, NeededSkill, EnabledTerm, Characteristic,
                atk[0], atk[1], Energy, (byte)Hit,
                (byte)Critical, Bullet, Spirit, BGM,
                (byte)MaximumRange, (byte)MinimumRange, UnitGroupOfUnion, PilotGroupOfUnion,
                SpecialEffecive, BulletFlag, RaisedPoint, AdaptationSky,
                AdaptationLand, AdaptationSea, AdaptationCosmos, Unknown,
            };
        }

        public int[] GetNameAddresses() => new int[] { NamePointer1, NamePointer2 };
    }
}
