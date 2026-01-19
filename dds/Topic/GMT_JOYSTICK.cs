/*
WARNING: THIS FILE IS AUTO-GENERATED. DO NOT MODIFY.

This file was generated from GMT_JOYSTICK.idl
using RTI Code Generator (rtiddsgen) version 4.3.0.
The rtiddsgen tool is part of the RTI Connext DDS distribution.
For more information, type 'rtiddsgen -help' at a command shell
or consult the Code Generator User's Manual.
*/

public class JOYSTICK_OWNERSHIP_STATUS :  IEquatable<JOYSTICK_OWNERSHIP_STATUS>
{
    public global::ST_MSG_HEADER stMsgHeader { get; set; }
    public global::E_EQUIP_ID eOwnerMfc { get; set; }
    public global::E_SW_ID eOwnerSW { get; set; }
    public global::ST_TIME_INFO stAchieveTime { get; set; }

    public JOYSTICK_OWNERSHIP_STATUS()
    {
        stMsgHeader = new global::ST_MSG_HEADER();
        eOwnerMfc = (global::E_EQUIP_ID) (0);
        eOwnerSW = (global::E_SW_ID) (0);
        stAchieveTime = new global::ST_TIME_INFO();
    }

    public JOYSTICK_OWNERSHIP_STATUS(global::ST_MSG_HEADER  stMsgHeader, global::E_EQUIP_ID  eOwnerMfc, global::E_SW_ID  eOwnerSW, global::ST_TIME_INFO  stAchieveTime)
    {
        this.stMsgHeader = stMsgHeader;
        this.eOwnerMfc = eOwnerMfc;
        this.eOwnerSW = eOwnerSW;
        this.stAchieveTime = stAchieveTime;
    }

    public JOYSTICK_OWNERSHIP_STATUS(JOYSTICK_OWNERSHIP_STATUS other)
    {
        if (other == null)
        {
            return;
        }

        this.stMsgHeader = new global::ST_MSG_HEADER(other.stMsgHeader);
        this.eOwnerMfc = other.eOwnerMfc;
        this.eOwnerSW = other.eOwnerSW;
        this.stAchieveTime = new global::ST_TIME_INFO(other.stAchieveTime);

    }

    public override int GetHashCode()
    {
        var hash = new HashCode();

        hash.Add(this.stMsgHeader);
        hash.Add(this.eOwnerMfc);
        hash.Add(this.eOwnerSW);
        hash.Add(this.stAchieveTime);

        return hash.ToHashCode();
    }

    public bool Equals(JOYSTICK_OWNERSHIP_STATUS other)
    {
        if (other == null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return this.stMsgHeader.Equals(other.stMsgHeader) && 
        this.eOwnerMfc.Equals(other.eOwnerMfc) && 
        this.eOwnerSW.Equals(other.eOwnerSW) && 
        this.stAchieveTime.Equals(other.stAchieveTime);
    }

    public override bool Equals(object obj) => this.Equals(obj as JOYSTICK_OWNERSHIP_STATUS);

    public override string ToString() => JOYSTICK_OWNERSHIP_STATUSSupport.Instance.ToString(this);
}

public class JOYSTICK_OWNERSHIP_COMMAND :  IEquatable<JOYSTICK_OWNERSHIP_COMMAND>
{
    public global::ST_MSG_HEADER stMsgHeader { get; set; }
    public global::E_EQUIP_ID eHeirMfc { get; set; }
    public global::E_SW_ID eHeirSW { get; set; }
    public global::ST_TIME_INFO stClaimTime { get; set; }

    public JOYSTICK_OWNERSHIP_COMMAND()
    {
        stMsgHeader = new global::ST_MSG_HEADER();
        eHeirMfc = (global::E_EQUIP_ID) (0);
        eHeirSW = (global::E_SW_ID) (0);
        stClaimTime = new global::ST_TIME_INFO();
    }

    public JOYSTICK_OWNERSHIP_COMMAND(global::ST_MSG_HEADER  stMsgHeader, global::E_EQUIP_ID  eHeirMfc, global::E_SW_ID  eHeirSW, global::ST_TIME_INFO  stClaimTime)
    {
        this.stMsgHeader = stMsgHeader;
        this.eHeirMfc = eHeirMfc;
        this.eHeirSW = eHeirSW;
        this.stClaimTime = stClaimTime;
    }

    public JOYSTICK_OWNERSHIP_COMMAND(JOYSTICK_OWNERSHIP_COMMAND other)
    {
        if (other == null)
        {
            return;
        }

        this.stMsgHeader = new global::ST_MSG_HEADER(other.stMsgHeader);
        this.eHeirMfc = other.eHeirMfc;
        this.eHeirSW = other.eHeirSW;
        this.stClaimTime = new global::ST_TIME_INFO(other.stClaimTime);

    }

    public override int GetHashCode()
    {
        var hash = new HashCode();

        hash.Add(this.stMsgHeader);
        hash.Add(this.eHeirMfc);
        hash.Add(this.eHeirSW);
        hash.Add(this.stClaimTime);

        return hash.ToHashCode();
    }

    public bool Equals(JOYSTICK_OWNERSHIP_COMMAND other)
    {
        if (other == null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return this.stMsgHeader.Equals(other.stMsgHeader) && 
        this.eHeirMfc.Equals(other.eHeirMfc) && 
        this.eHeirSW.Equals(other.eHeirSW) && 
        this.stClaimTime.Equals(other.stClaimTime);
    }

    public override bool Equals(object obj) => this.Equals(obj as JOYSTICK_OWNERSHIP_COMMAND);

    public override string ToString() => JOYSTICK_OWNERSHIP_COMMANDSupport.Instance.ToString(this);
}

public class JOYSTICK_OWNERSHIP_DROP_COMMAND :  IEquatable<JOYSTICK_OWNERSHIP_DROP_COMMAND>
{
    public global::ST_MSG_HEADER stMsgHeader { get; set; }
    public global::E_EQUIP_ID eDropMfc { get; set; }
    public global::E_SW_ID eDropSW { get; set; }
    public global::ST_TIME_INFO stDropTime { get; set; }

    public JOYSTICK_OWNERSHIP_DROP_COMMAND()
    {
        stMsgHeader = new global::ST_MSG_HEADER();
        eDropMfc = (global::E_EQUIP_ID) (0);
        eDropSW = (global::E_SW_ID) (0);
        stDropTime = new global::ST_TIME_INFO();
    }

    public JOYSTICK_OWNERSHIP_DROP_COMMAND(global::ST_MSG_HEADER  stMsgHeader, global::E_EQUIP_ID  eDropMfc, global::E_SW_ID  eDropSW, global::ST_TIME_INFO  stDropTime)
    {
        this.stMsgHeader = stMsgHeader;
        this.eDropMfc = eDropMfc;
        this.eDropSW = eDropSW;
        this.stDropTime = stDropTime;
    }

    public JOYSTICK_OWNERSHIP_DROP_COMMAND(JOYSTICK_OWNERSHIP_DROP_COMMAND other)
    {
        if (other == null)
        {
            return;
        }

        this.stMsgHeader = new global::ST_MSG_HEADER(other.stMsgHeader);
        this.eDropMfc = other.eDropMfc;
        this.eDropSW = other.eDropSW;
        this.stDropTime = new global::ST_TIME_INFO(other.stDropTime);

    }

    public override int GetHashCode()
    {
        var hash = new HashCode();

        hash.Add(this.stMsgHeader);
        hash.Add(this.eDropMfc);
        hash.Add(this.eDropSW);
        hash.Add(this.stDropTime);

        return hash.ToHashCode();
    }

    public bool Equals(JOYSTICK_OWNERSHIP_DROP_COMMAND other)
    {
        if (other == null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return this.stMsgHeader.Equals(other.stMsgHeader) && 
        this.eDropMfc.Equals(other.eDropMfc) && 
        this.eDropSW.Equals(other.eDropSW) && 
        this.stDropTime.Equals(other.stDropTime);
    }

    public override bool Equals(object obj) => this.Equals(obj as JOYSTICK_OWNERSHIP_DROP_COMMAND);

    public override string ToString() => JOYSTICK_OWNERSHIP_DROP_COMMANDSupport.Instance.ToString(this);
}

