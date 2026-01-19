using System.Reflection;
using System.Runtime.InteropServices;

namespace Pa5455CmsDds.Component.Joystick
{
    public static class WindowsApi
    {
        [Flags]
        public enum DIGCF
        {
            NONE = 0,
            DEFAULT = 1,
            PRESENT = 2,
            ALL_CLASSES = 4,
            PROFILE = 8,
            DEVICE_INTERFACE = 16,
        }

        [Flags]
        public enum SPINT
        {
            NONE = 0,
            ACTIVE = 1,
            DEFAULT = 2,
            REMOVED = 4,
        }

        [Flags]
        public enum FILE_ACCESS : uint
        {
            NONE = 0,
            READ = 0x80000000,
            WRITE = 0x40000000,
            EXECUTE = 0x20000000,
            ALL = 0x10000000,
        }

        [Flags]
        public enum FILE_SHARE : uint
        {
            NONE = 0x00000000,
            READ = 0x00000001,
            WRITE = 0x00000002,
            DELETE = 0x00000004,
            ALL = READ | WRITE | DELETE,
        }

        public enum CREATION_DISPOSITION : uint
        {
            NEW = 1,
            CREATE_ALWAYS = 2,
            OPEN_EXISTING = 3,
            OPEN_ALWAYS = 4,
            TRUNCATE_EXISTING = 5,
        }

        [Flags]
        public enum FILE_ATTRIBUTES : uint
        {
            READ_ONLY = 0x00000001,
            HIDDEN = 0x00000002,
            SYSTEM = 0x00000004,
            DIRECTORY = 0x00000010,
            ARCHIVE = 0x00000020,
            DEVICE = 0x00000040,
            NORMAL = 0x00000080,
            TEMPORARY = 0x00000100,
            SPARSE_FILE = 0x00000200,
            REPARSE_POINT = 0x00000400,
            COMPRESSED = 0x00000800,
            OFFLINE = 0x00001000,
            NOT_CONTENT_INDEXED = 0x00002000,
            ENCRYPTED = 0x00004000,
            WRITETHROUGH = 0x80000000,
            OVERLAPPED = 0x40000000,
            NO_BUFFERING = 0x20000000,
            RANDOM_ACCESS = 0x10000000,
            SEQUENTIAL_SCAN = 0x08000000,
            DELETE_ON_CLOSE = 0x04000000,
            BACKUP_SEMANTICS = 0x02000000,
            POSIX_SEMANTICS = 0x01000000,
            OPEN_REPARSE_POINT = 0x00200000,
            OPEN_NO_RECALL = 0x00100000,
            FIRST_PIPE_INSTANCE = 0x00080000,
        }



        public struct HDEVINFO
        {
            IntPtr Value;

            public void Invalidate()
            {
                Value = (IntPtr)(-1);
            }

            public bool IsValid
            {
                get { return Value != (IntPtr)(-1); }
            }
        }

        public struct SP_DEVINFO_DATA
        {
            public int Size;
            public Guid ClassGuid;
            public uint DevInst;
            IntPtr Reserved;
        }

        public struct SP_DEVICE_INTERFACE_DATA
        {
            public int Size;
            public Guid InterfaceClassGuid;
            public SPINT Flags;
            IntPtr Reserved;
        }

        [Obfuscation(Exclude = true)]
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct SP_DEVICE_INTERFACE_DETAIL_DATA
        {
            public int Size;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
            public string DevicePath;
        }

        public struct HIDD_ATTRIBUTES
        {
            public int Size;
            public ushort VendorID, ProductID, VersionNumber;
        }


        public class DeviceInterface
        {
            public string id;
            public string path;
        }

        public class DeviceAttibute
        {
            public ushort pid;
            public ushort vid;
            public ushort version;
        }



        #region [[ hid.dll ]]
        [DllImport("hid.dll")]
        public static extern void HidD_GetHidGuid(out Guid hidGuid);

        [DllImport("hid.dll")]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool HidD_GetAttributes(IntPtr handle, ref HIDD_ATTRIBUTES attributes);

        [DllImport("hid.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool HidD_SetNumInputBuffers(IntPtr handle, int count);
        #endregion

        #region [[ setupapi.dll ]]
        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern HDEVINFO SetupDiGetClassDevs(
            [MarshalAs(UnmanagedType.LPStruct)] Guid classGuid,
            string enumerator,
            IntPtr hwndParent,
            DIGCF flags
        );

        [DllImport("setupapi.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetupDiEnumDeviceInfo(
            HDEVINFO deviceInfoSet,
            int memberIndex,
            ref SP_DEVINFO_DATA deviceInfoData
        );

        [DllImport("setupapi.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetupDiEnumDeviceInterfaces(
            HDEVINFO deviceInfoSet,
            ref SP_DEVINFO_DATA deviceInfoData,
            [MarshalAs(UnmanagedType.LPStruct)] Guid interfaceClassGuid,
            int memberIndex,
            ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData
        );

        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetupDiGetDeviceInterfaceDetail(
            HDEVINFO deviceInfoSet,
            ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData,
            ref SP_DEVICE_INTERFACE_DETAIL_DATA deviceInterfaceDetailData,
            int deviceInterfaceDetailDataSize,
            IntPtr requiredSize,
            IntPtr deviceInfoData
        );
        #endregion

        #region [[ cfgmgr32.dll ]]
        [DllImport("cfgmgr32.dll")]
        public static extern int CM_Get_Child(out uint childDevInst, uint devInst, int flags = 0);

        public static int CM_Get_Device_ID(uint devInst, out string deviceID)
        {
            int result = CM_Get_Device_ID_Size(out int length, devInst);
            
            if (result != 0)
            {
                deviceID = "";

                return result;
            }

            char[] charBuffer = new char[length + 1];
            result = CM_Get_Device_ID(devInst, charBuffer, charBuffer.Length);

            if (result != 0)
            {
                deviceID = "";

                return result;
            }

            deviceID = new string(charBuffer, 0, length);

            return 0;
        }

        [DllImport("cfgmgr32.dll", CharSet = CharSet.Unicode)]
        public static extern int CM_Get_Device_ID(uint devInst, char[] buffer, int length, int flags = 0);

        [DllImport("cfgmgr32.dll")]
        public static extern int CM_Get_Device_ID_Size(out int length, uint devInst, int flags = 0);
        #endregion

        #region [[ kernel32.dll ]]
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr handle);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr CreateFile(
            string filename,
            FILE_ACCESS desiredAccess,
            FILE_SHARE shareMode,
            IntPtr securityAttributes,
            CREATION_DISPOSITION creationDisposition,
            FILE_ATTRIBUTES attributes,
            IntPtr template
        );

        public static IntPtr CreateFileFromDevice(string filename, FILE_ACCESS desiredAccess, FILE_SHARE shareMode)
        {
            return CreateFile(
                filename,
                desiredAccess,
                shareMode,
                IntPtr.Zero,
                CREATION_DISPOSITION.OPEN_EXISTING,
                FILE_ATTRIBUTES.DEVICE | FILE_ATTRIBUTES.OVERLAPPED,
                IntPtr.Zero
            );
        }

        public static bool TryOpenToGetInfo(string path, Func<IntPtr, bool> action)
        {
            var handle = CreateFileFromDevice(path, FILE_ACCESS.NONE, FILE_SHARE.READ | FILE_SHARE.WRITE);

            if (handle == (IntPtr)(-1)) { return false; }

            try
            {
                return action(handle);
            }
            catch (Exception e)
            {
                Exception debug = e;
            }
            finally
            {
                CloseHandle(handle);
            }

            return false;
        }
        #endregion


        public static List<DeviceInterface> getEnumerateHidInterfaces(Guid hidGuid)
        {
            List<DeviceInterface> deviceInterfaceCollection = new List<DeviceInterface>();

            HDEVINFO deviceInformation = SetupDiGetClassDevs(
                hidGuid,
                null,
                IntPtr.Zero,
                DIGCF.DEVICE_INTERFACE | DIGCF.PRESENT
            );

            if (deviceInformation.IsValid)
            {
                try
                {
                    SP_DEVINFO_DATA dvi = new SP_DEVINFO_DATA();
                    dvi.Size = Marshal.SizeOf(dvi);

                    for (int i = 0; SetupDiEnumDeviceInfo(deviceInformation, i, ref dvi); i++)
                    {
                        string deviceId;

                        if (CM_Get_Device_ID(dvi.DevInst, out deviceId) != 0) { continue; }

                        SP_DEVICE_INTERFACE_DATA did = new SP_DEVICE_INTERFACE_DATA();
                        did.Size = Marshal.SizeOf(did);

                        for (int j = 0; SetupDiEnumDeviceInterfaces(deviceInformation, ref dvi, hidGuid, j, ref did); j++)
                        {
                            string devicePath;

                            if (setupDiGetDeviceInterfaceDevicePath(deviceInformation, ref did, out devicePath))
                            {
                                deviceInterfaceCollection.Add(new DeviceInterface() {
                                    id = deviceId,
                                    path = devicePath,
                                });
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Exception debug = e;
                }
            }

            return deviceInterfaceCollection;
        }

        public static bool setupDiGetDeviceInterfaceDevicePath(HDEVINFO deviceInfoSet, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData, out string devicePath)
        {
            if (setupDiGetDeviceInterfaceDetail(deviceInfoSet, ref deviceInterfaceData, out SP_DEVICE_INTERFACE_DETAIL_DATA diDetail))
            {
                devicePath = diDetail.DevicePath;

                return true;
            }

            devicePath = "";
            
            return false;
        }

        public static bool setupDiGetDeviceInterfaceDetail(HDEVINFO deviceInfoSet, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData, out SP_DEVICE_INTERFACE_DETAIL_DATA deviceInterfaceDetailData)
        {
            deviceInterfaceDetailData = new SP_DEVICE_INTERFACE_DETAIL_DATA();
            deviceInterfaceDetailData.Size = ((IntPtr.Size == 8) ? 8 : (4 + Marshal.SystemDefaultCharSize));

            if (SetupDiGetDeviceInterfaceDetail(deviceInfoSet, ref deviceInterfaceData, ref deviceInterfaceDetailData, Marshal.SizeOf(deviceInterfaceDetailData) - 4, IntPtr.Zero, IntPtr.Zero))
            {
                return true;
            }
            else
            {
                deviceInterfaceDetailData = default(SP_DEVICE_INTERFACE_DETAIL_DATA);

                return false;
            }
        }

        public static DeviceAttibute getDeviceAttribute(string devicePath)
        {
            DeviceAttibute deviceAttibute = new DeviceAttibute();

            bool result = TryOpenToGetInfo(devicePath, handle => {
                HIDD_ATTRIBUTES attributes = new HIDD_ATTRIBUTES();
                attributes.Size = Marshal.SizeOf(attributes);

                if (!HidD_GetAttributes(handle, ref attributes))
                {
                    return false;
                }

                deviceAttibute.pid = attributes.ProductID;
                deviceAttibute.vid = attributes.VendorID;
                deviceAttibute.version = attributes.VersionNumber;

                return true;
            });

            return deviceAttibute;
        }

        public static IntPtr tryOpen(string path)
        {
            IntPtr handle = CreateFileFromDevice(path, FILE_ACCESS.READ | FILE_ACCESS.WRITE, FILE_SHARE.READ | FILE_SHARE.WRITE);

            if (handle != (IntPtr)(-1))
            {
                int maxInputBuffers = ((Environment.OSVersion.Version >= new Version(5, 1)) ? 512 : 200); // Windows 2000 supports 200. Windows XP supports 512.

                if (HidD_SetNumInputBuffers(handle, maxInputBuffers))
                {
                    IntPtr streamHandle = handle;

                    return streamHandle;
                }
                else
                {
                    CloseHandle(handle);

                    return (IntPtr)(-1);
                }
            }
            else
            {
                return handle;
            }
        }
    }
}