/*
WARNING: THIS FILE IS AUTO-GENERATED. DO NOT MODIFY.

This file was generated from GMT_JOYSTICK.idl
using RTI Code Generator (rtiddsgen) version 4.3.0.
The rtiddsgen tool is part of the RTI Connext DDS distribution.
For more information, type 'rtiddsgen -help' at a command shell
or consult the Code Generator User's Manual.
*/

using Omg.Types;
using Rti.Dds.Core;
using Rti.Types.Dynamic;

namespace Implementation
{

    public struct JOYSTICK_OWNERSHIP_STATUSUnmanaged : Rti.Dds.NativeInterface.TypePlugin.INativeTopicType<global::JOYSTICK_OWNERSHIP_STATUS>
    {

        private global::Implementation.ST_MSG_HEADERUnmanaged stMsgHeader;
        private global::E_EQUIP_ID eOwnerMfc;
        private global::E_SW_ID eOwnerSW;
        private global::Implementation.ST_TIME_INFOUnmanaged stAchieveTime;

        public void Destroy(bool optionalsOnly)
        {
            if (optionalsOnly)
            {
                return;
            }
            stMsgHeader.Destroy(optionalsOnly);
            stAchieveTime.Destroy(optionalsOnly);
        }

        public void FromNative(global::JOYSTICK_OWNERSHIP_STATUS sample, bool keysOnly = false)
        {

            stMsgHeader.FromNative(sample.stMsgHeader, keysOnly: false);
            sample.eOwnerMfc = eOwnerMfc;
            sample.eOwnerSW = eOwnerSW;
            stAchieveTime.FromNative(sample.stAchieveTime, keysOnly: false);
        }

        public void Initialize(bool allocatePointers = true, bool allocateMemory = true)
        {
            stMsgHeader.Initialize(allocatePointers, allocateMemory);
            eOwnerMfc = (global::E_EQUIP_ID) (0);
            eOwnerSW = (global::E_SW_ID) (0);
            stAchieveTime.Initialize(allocatePointers, allocateMemory);
        }

        public void ToNative(global::JOYSTICK_OWNERSHIP_STATUS sample, bool keysOnly = false)
        {
            stMsgHeader.ToNative(sample.stMsgHeader, keysOnly: false);
            eOwnerMfc = sample.eOwnerMfc;
            eOwnerSW = sample.eOwnerSW;
            stAchieveTime.ToNative(sample.stAchieveTime, keysOnly: false);
        }
    }

    internal class JOYSTICK_OWNERSHIP_STATUSPlugin : Rti.Dds.NativeInterface.TypePlugin.InterpretedTypePlugin<global::JOYSTICK_OWNERSHIP_STATUS, JOYSTICK_OWNERSHIP_STATUSUnmanaged>
    {

        internal JOYSTICK_OWNERSHIP_STATUSPlugin() : base("global::JOYSTICK_OWNERSHIP_STATUS", isKeyed: false, CreateDynamicType(isPublic: false))
        {
        }

        public static DynamicType CreateDynamicType(bool isPublic = true)
        {
            var dtf = ServiceEnvironment.Instance.Internal.GetTypeFactory(isPublic);
            var tsf = ServiceEnvironment.Instance.Internal.TypeSupportFactory;

            // JOYSTICK_OWNERSHIP_STATUS struct
            var JOYSTICK_OWNERSHIP_STATUSStructMembers = new StructMember[]
            {
                new StructMember("stMsgHeader", global::ST_MSG_HEADERSupport.Instance.GetDynamicTypeInternal(isPublic), id: 0),
                new StructMember("eOwnerMfc", global::E_EQUIP_IDSupport.Instance.GetDynamicTypeInternal(isPublic), id: 1),
                new StructMember("eOwnerSW", global::E_SW_IDSupport.Instance.GetDynamicTypeInternal(isPublic), id: 2),
                new StructMember("stAchieveTime", global::ST_TIME_INFOSupport.Instance.GetDynamicTypeInternal(isPublic), id: 3)
            };

            DynamicType result = tsf.CreateTypeWithAccessInfo<JOYSTICK_OWNERSHIP_STATUSUnmanaged>(
                dtf.BuildStruct()
                .WithExtensibility(ExtensibilityKind.Extensible)
                .WithName("JOYSTICK_OWNERSHIP_STATUS")
                .AddMembers(JOYSTICK_OWNERSHIP_STATUSStructMembers));

            return result;
        }
    }
}
public class JOYSTICK_OWNERSHIP_STATUSSupport : Rti.Dds.Topics.TypeSupport<global::JOYSTICK_OWNERSHIP_STATUS>
{
    public JOYSTICK_OWNERSHIP_STATUSSupport() : base(
        new Implementation.JOYSTICK_OWNERSHIP_STATUSPlugin(),
        new Lazy<DynamicType>(() =>Implementation.JOYSTICK_OWNERSHIP_STATUSPlugin.CreateDynamicType(isPublic: true)))
    {
    }

    public static JOYSTICK_OWNERSHIP_STATUSSupport Instance { get; } =
    ServiceEnvironment.Instance.Internal.TypeSupportFactory.CreateTypeSupport<JOYSTICK_OWNERSHIP_STATUSSupport, global::JOYSTICK_OWNERSHIP_STATUS>();

}

namespace Implementation
{

    public struct JOYSTICK_OWNERSHIP_COMMANDUnmanaged : Rti.Dds.NativeInterface.TypePlugin.INativeTopicType<global::JOYSTICK_OWNERSHIP_COMMAND>
    {

        private global::Implementation.ST_MSG_HEADERUnmanaged stMsgHeader;
        private global::E_EQUIP_ID eHeirMfc;
        private global::E_SW_ID eHeirSW;
        private global::Implementation.ST_TIME_INFOUnmanaged stClaimTime;

        public void Destroy(bool optionalsOnly)
        {
            if (optionalsOnly)
            {
                return;
            }
            stMsgHeader.Destroy(optionalsOnly);
            stClaimTime.Destroy(optionalsOnly);
        }

        public void FromNative(global::JOYSTICK_OWNERSHIP_COMMAND sample, bool keysOnly = false)
        {

            stMsgHeader.FromNative(sample.stMsgHeader, keysOnly: false);
            sample.eHeirMfc = eHeirMfc;
            sample.eHeirSW = eHeirSW;
            stClaimTime.FromNative(sample.stClaimTime, keysOnly: false);
        }

        public void Initialize(bool allocatePointers = true, bool allocateMemory = true)
        {
            stMsgHeader.Initialize(allocatePointers, allocateMemory);
            eHeirMfc = (global::E_EQUIP_ID) (0);
            eHeirSW = (global::E_SW_ID) (0);
            stClaimTime.Initialize(allocatePointers, allocateMemory);
        }

        public void ToNative(global::JOYSTICK_OWNERSHIP_COMMAND sample, bool keysOnly = false)
        {
            stMsgHeader.ToNative(sample.stMsgHeader, keysOnly: false);
            eHeirMfc = sample.eHeirMfc;
            eHeirSW = sample.eHeirSW;
            stClaimTime.ToNative(sample.stClaimTime, keysOnly: false);
        }
    }

    internal class JOYSTICK_OWNERSHIP_COMMANDPlugin : Rti.Dds.NativeInterface.TypePlugin.InterpretedTypePlugin<global::JOYSTICK_OWNERSHIP_COMMAND, JOYSTICK_OWNERSHIP_COMMANDUnmanaged>
    {

        internal JOYSTICK_OWNERSHIP_COMMANDPlugin() : base("global::JOYSTICK_OWNERSHIP_COMMAND", isKeyed: false, CreateDynamicType(isPublic: false))
        {
        }

        public static DynamicType CreateDynamicType(bool isPublic = true)
        {
            var dtf = ServiceEnvironment.Instance.Internal.GetTypeFactory(isPublic);
            var tsf = ServiceEnvironment.Instance.Internal.TypeSupportFactory;

            // JOYSTICK_OWNERSHIP_COMMAND struct
            var JOYSTICK_OWNERSHIP_COMMANDStructMembers = new StructMember[]
            {
                new StructMember("stMsgHeader", global::ST_MSG_HEADERSupport.Instance.GetDynamicTypeInternal(isPublic), id: 0),
                new StructMember("eHeirMfc", global::E_EQUIP_IDSupport.Instance.GetDynamicTypeInternal(isPublic), id: 1),
                new StructMember("eHeirSW", global::E_SW_IDSupport.Instance.GetDynamicTypeInternal(isPublic), id: 2),
                new StructMember("stClaimTime", global::ST_TIME_INFOSupport.Instance.GetDynamicTypeInternal(isPublic), id: 3)
            };

            DynamicType result = tsf.CreateTypeWithAccessInfo<JOYSTICK_OWNERSHIP_COMMANDUnmanaged>(
                dtf.BuildStruct()
                .WithExtensibility(ExtensibilityKind.Extensible)
                .WithName("JOYSTICK_OWNERSHIP_COMMAND")
                .AddMembers(JOYSTICK_OWNERSHIP_COMMANDStructMembers));

            return result;
        }
    }
}
public class JOYSTICK_OWNERSHIP_COMMANDSupport : Rti.Dds.Topics.TypeSupport<global::JOYSTICK_OWNERSHIP_COMMAND>
{
    public JOYSTICK_OWNERSHIP_COMMANDSupport() : base(
        new Implementation.JOYSTICK_OWNERSHIP_COMMANDPlugin(),
        new Lazy<DynamicType>(() =>Implementation.JOYSTICK_OWNERSHIP_COMMANDPlugin.CreateDynamicType(isPublic: true)))
    {
    }

    public static JOYSTICK_OWNERSHIP_COMMANDSupport Instance { get; } =
    ServiceEnvironment.Instance.Internal.TypeSupportFactory.CreateTypeSupport<JOYSTICK_OWNERSHIP_COMMANDSupport, global::JOYSTICK_OWNERSHIP_COMMAND>();

}

namespace Implementation
{

    public struct JOYSTICK_OWNERSHIP_DROP_COMMANDUnmanaged : Rti.Dds.NativeInterface.TypePlugin.INativeTopicType<global::JOYSTICK_OWNERSHIP_DROP_COMMAND>
    {

        private global::Implementation.ST_MSG_HEADERUnmanaged stMsgHeader;
        private global::E_EQUIP_ID eDropMfc;
        private global::E_SW_ID eDropSW;
        private global::Implementation.ST_TIME_INFOUnmanaged stDropTime;

        public void Destroy(bool optionalsOnly)
        {
            if (optionalsOnly)
            {
                return;
            }
            stMsgHeader.Destroy(optionalsOnly);
            stDropTime.Destroy(optionalsOnly);
        }

        public void FromNative(global::JOYSTICK_OWNERSHIP_DROP_COMMAND sample, bool keysOnly = false)
        {

            stMsgHeader.FromNative(sample.stMsgHeader, keysOnly: false);
            sample.eDropMfc = eDropMfc;
            sample.eDropSW = eDropSW;
            stDropTime.FromNative(sample.stDropTime, keysOnly: false);
        }

        public void Initialize(bool allocatePointers = true, bool allocateMemory = true)
        {
            stMsgHeader.Initialize(allocatePointers, allocateMemory);
            eDropMfc = (global::E_EQUIP_ID) (0);
            eDropSW = (global::E_SW_ID) (0);
            stDropTime.Initialize(allocatePointers, allocateMemory);
        }

        public void ToNative(global::JOYSTICK_OWNERSHIP_DROP_COMMAND sample, bool keysOnly = false)
        {
            stMsgHeader.ToNative(sample.stMsgHeader, keysOnly: false);
            eDropMfc = sample.eDropMfc;
            eDropSW = sample.eDropSW;
            stDropTime.ToNative(sample.stDropTime, keysOnly: false);
        }
    }

    internal class JOYSTICK_OWNERSHIP_DROP_COMMANDPlugin : Rti.Dds.NativeInterface.TypePlugin.InterpretedTypePlugin<global::JOYSTICK_OWNERSHIP_DROP_COMMAND, JOYSTICK_OWNERSHIP_DROP_COMMANDUnmanaged>
    {

        internal JOYSTICK_OWNERSHIP_DROP_COMMANDPlugin() : base("global::JOYSTICK_OWNERSHIP_DROP_COMMAND", isKeyed: false, CreateDynamicType(isPublic: false))
        {
        }

        public static DynamicType CreateDynamicType(bool isPublic = true)
        {
            var dtf = ServiceEnvironment.Instance.Internal.GetTypeFactory(isPublic);
            var tsf = ServiceEnvironment.Instance.Internal.TypeSupportFactory;

            // JOYSTICK_OWNERSHIP_DROP_COMMAND struct
            var JOYSTICK_OWNERSHIP_DROP_COMMANDStructMembers = new StructMember[]
            {
                new StructMember("stMsgHeader", global::ST_MSG_HEADERSupport.Instance.GetDynamicTypeInternal(isPublic), id: 0),
                new StructMember("eDropMfc", global::E_EQUIP_IDSupport.Instance.GetDynamicTypeInternal(isPublic), id: 1),
                new StructMember("eDropSW", global::E_SW_IDSupport.Instance.GetDynamicTypeInternal(isPublic), id: 2),
                new StructMember("stDropTime", global::ST_TIME_INFOSupport.Instance.GetDynamicTypeInternal(isPublic), id: 3)
            };

            DynamicType result = tsf.CreateTypeWithAccessInfo<JOYSTICK_OWNERSHIP_DROP_COMMANDUnmanaged>(
                dtf.BuildStruct()
                .WithExtensibility(ExtensibilityKind.Extensible)
                .WithName("JOYSTICK_OWNERSHIP_DROP_COMMAND")
                .AddMembers(JOYSTICK_OWNERSHIP_DROP_COMMANDStructMembers));

            return result;
        }
    }
}
public class JOYSTICK_OWNERSHIP_DROP_COMMANDSupport : Rti.Dds.Topics.TypeSupport<global::JOYSTICK_OWNERSHIP_DROP_COMMAND>
{
    public JOYSTICK_OWNERSHIP_DROP_COMMANDSupport() : base(
        new Implementation.JOYSTICK_OWNERSHIP_DROP_COMMANDPlugin(),
        new Lazy<DynamicType>(() =>Implementation.JOYSTICK_OWNERSHIP_DROP_COMMANDPlugin.CreateDynamicType(isPublic: true)))
    {
    }

    public static JOYSTICK_OWNERSHIP_DROP_COMMANDSupport Instance { get; } =
    ServiceEnvironment.Instance.Internal.TypeSupportFactory.CreateTypeSupport<JOYSTICK_OWNERSHIP_DROP_COMMANDSupport, global::JOYSTICK_OWNERSHIP_DROP_COMMAND>();

}

