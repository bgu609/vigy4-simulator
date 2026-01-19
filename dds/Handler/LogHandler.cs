using System;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Pa5455CmsDds.Handler
{
    public static class LogHandler
    {
        // 토글 & 레벨
        public static volatile bool Enabled = true;

        public enum Enum_DebugStatus
        {
            None = 0,  // no output
            Error = 1, // errors
            Info = 2,  // info
            All = 3   // verbose
        }

        public static Enum_DebugStatus DebugLevel { get; set; } = Enum_DebugStatus.All;

        // 기본 싱크: Trace
        public static Action<string> Sink { get; set; } = s => Trace.WriteLine(s);

        // ========= Public APIs =========
        public static void LogPub<T>(Enum_DebugStatus level, T msg, string? note = null) where T : IEquatable<T>
        {
            if (!Enabled || !ShouldLog(level)) return;
            Sink(Format(level, "Pub", typeof(T).Name, msg!, note));
        }

        public static void LogSub<T>(Enum_DebugStatus level, T msg, string? note = null) where T : IEquatable<T>
        {
            if (!Enabled || !ShouldLog(level)) return;
            Sink(Format(level, "Sub", typeof(T).Name, msg!, note));
        }

        public static void LogEtc(Enum_DebugStatus level, string msg)
        {
            if (!Enabled || !ShouldLog(level)) return;
            Sink(Format(level, msg));
        }

        // ========= Helpers =========
        private static bool ShouldLog(Enum_DebugStatus level)
            => level <= DebugLevel; // None(0) < Error(1) < Info(2) < All(3)

        private static string SafeStr(char[]? chars)
            => chars == null ? string.Empty : new string(chars).TrimEnd('\0', ' ');

        private static string FmtTime(ST_TIME_INFO t)
            => $"{t.unYear:0000}-{t.byMonth:00}-{t.byDay:00} {t.byHour:00}:{t.byMin:00}.{t.usMSec:000}";

        private static string Format<T>(Enum_DebugStatus level, string dir, string topic, T payload)
            => Format(level, dir, topic, payload, null);

        private static string Format<T>(Enum_DebugStatus level, string dir, string topic, T payload, string? note)
        {
            var sb = new StringBuilder(512);
            sb.Append('[').Append(DateTime.Now.ToString("HH:mm:ss.fff")).Append("] | ");
            sb.Append('[').Append(level).Append(' ').Append(dir).Append(' ').Append(topic).Append(']');
            if (!string.IsNullOrEmpty(note)) sb.Append(" (").Append(note).Append(')');
            sb.Append(" | ");

            switch (payload)
            {
                // ===== 공통/시스템 =====
                case ST_MSG_HEADER h:
                    sb.Append($"send={h.eSendEquipID}/{h.eSendDevID}/{h.eSendSW}/{h.eRedundancy}, topic={h.eTopicID}, ts={FmtTime(h.stSendTime)}");
                    break;

                case CMS_REDUNDANCY m:
                    sb.Append($"equip={m.eEquipID}, dev={m.eDevID}, dual={m.eDual}, prio={m.byPriority}");
                    break;

                //case CMS_DIAGNOSIS_RESULT m:
                //    {
                //        var count = m.stDiagnosisResult?.Count ?? 0;
                //        var samples = m.stDiagnosisResult?.Take(3)
                //            .Select(d => $"{d.eCategory1}:{SafeStr(d.strContents)}={d.usValue}({d.eInformType})")
                //            .ToArray() ?? Array.Empty<string>();
                //        sb.Append($"equip={m.eEquipmentID}, dev={m.eDeviceID}, items={count}");
                //        if (samples.Length > 0) sb.Append($", sample=[{string.Join("; ", samples)}]");
                //    }
                //    break;

                case CMS_SW_STATUS m:
                    sb.Append($"app='{SafeStr(m.strApplicationName)}', build={FmtTime(m.stBuildDate)}, mode={m.eSwMode}, status={m.eSwStatus}");
                    break;

                case CMS_ALERT_REPORT m:
                    sb.Append($"upd={m.byUpdateType}, target={m.stAlert.eTargetEquip}, type={m.stAlert.eType}, cat={m.stAlert.eCategory}, ")
                      .Append($"sw={m.stAlert.eSendSwID}, id={m.stAlert.unAlertID}, ")
                      .Append($"msg='{SafeStr(m.stAlert.szMsg)}', at={FmtTime(m.stAlert.stDateTime)}, ack={m.stAlert.eCheck}");
                    break;

                case SM_SS_SYSTEM_STATUS m:
                    sb.Append($"mode={m.eSystemMode}, BS={m.eSystemBattleShort}, ENG={m.eEngagmentStatus}, ")
                      .Append($"timeSync={m.stTimeSyncInfo.eTimeSync}, ts={FmtTime(m.stTimeSyncInfo.stDateTime)}");
                    break;

                case OSS_OWN_SHIP_DATA m:
                    sb.Append($"HDG={m.fHeading:F1}°, Pitch={m.fPitch:F1}°, Roll={m.fRoll:F1}°, ")
                      .Append($"COG={m.fCourseOverGround:F1}°, SOG={m.fSpeedOverGround:F1}kt, ")
                      .Append($"Lat={m.dLatitude:F6}, Lon={m.dLongitude:F6}, Depth={m.fWaterDepth:F1}m, ")
                      .Append($"WindDir={m.fWindDirection:F0}°, Wind={m.fWindSpeed:F1}m/s, ")
                      .Append($"Gyro={m.nGyroAvail}/{m.nGyroDataMode}, GPS={m.nGPSAvail}/{m.nGPSDataMode}");
                    break;

                case CMS_ISA_ANALYSIS_MESSAGE m:
                    sb.Append($"analysis bytes={m.byAnalysisContents?.Count ?? 0}");
                    break;

                // ===== 장비 연결 제어 =====
                case MFC_EQUIP_CONNECTION_CTRL m:
                    sb.Append($"sensor={m.eSensorID}, order={m.eConnectionOrder}");
                    break;

                // ===== RESM =====
                case RESM_SYSTEM_STATUS m:
                    sb.Append($"equip={m.eEquipmentID}, conn={m.eConnectionStatus}, ES={m.eESStatus}, EC={m.eECStatus}, PT={m.ePTStatus}, ST={m.eSTStatus}, sw={m.eSwStatus}");
                    break;

                case RESM_ECM_MANUAL_MODE_STATUS m:
                    sb.Append($"manual={m.eManualMode}, jam={m.eJammingStatus}, freqMHz={m.fFreq:F0}, brg={m.fBearing:F1}");
                    break;

                case RESM_TRACK_INFO m:
                    sb.Append($"TTN={m.unTTN}, ESMNo={m.unESMTrackNo}, valid={m.eValidity}, brg={m.fBearing:F1}, ")
                      .Append($"Freq={m.stRESMDetail.fFreq:F0}MHz, PRI={m.stRESMDetail.fPRI:F1}x0.1us, PW={m.stRESMDetail.fPulseWidth:F1}x0.1us, ")
                      .Append($"Scan={m.stRESMDetail.eScanType}, Threat={m.stRESMDetail.unThreatLevel}, ")
                      .Append($"Emitter='{SafeStr(m.stRESMDetail.chEmitterID)}'");
                    break;

                case RESM_BEARING_REPORT m:
                    sb.Append($"TTN={m.unTTN}, ESMNo={m.unESMTrackNo}, valid={m.eValidity}, brg={m.fBearing:F1}, emitterID='{SafeStr(m.chEmitterID)}'");
                    break;

                case RESM_TRACK_DELETE m:
                    sb.Append($"TTN={m.unTTN}");
                    break;

                case MFC_SENSD_RESM_SIGNAL_REPORT_REQ m:
                    sb.Append($"TTN={m.unTTN}");
                    break;

                case MFC_SENSD_RESM_BEARING_REPORT_REQ m:
                    sb.Append($"start={m.fStartBearing:F1}, end={m.fEndBearing:F1}");
                    break;

                case MFC_SENSD_RESM_ECM_CTRL m:
                    sb.Append($"TTN={m.unTTN}, jamOrder={m.eJammingOrder}");
                    break;

                // ===== CESM =====
                case CESM_SYSTEM_STATUS m:
                    sb.Append($"equip={m.eEquipmentID}, conn={m.eConnectionStatus}, CSD={m.eCSDStatus}, CSP={m.eCSPStatus}, HSP={m.eHSPStatus}, sw={m.eSwStatus}");
                    break;

                case CESM_TRACK_INFO m:
                    sb.Append($"TTN={m.unTTN}, ESMNo={m.unESMTrackNo}, valid={m.eValidity}, brg={m.fBearing:F1}, ")
                      .Append($"F={m.stCESMDetail.fFreq:F1}MHz, Amp={m.stCESMDetail.fAmp:F1}, Mod={m.stCESMDetail.eModulation}");
                    break;

                case CESM_BEARING_REPORT m:
                    sb.Append($"TTN={m.unTTN}, ESMNo={m.unESMTrackNo}, valid={m.eValidity}, brg={m.fBearing:F1}");
                    break;

                case CESM_TRACK_DELETE m:
                    sb.Append($"TTN={m.unTTN}");
                    break;

                case MFC_SENSD_CESM_SIGNAL_REPORT_REQ m:
                    sb.Append($"TTN={m.unTTN}");
                    break;

                case MFC_SENSD_CESM_BEARING_REPORT_REQ m:
                    sb.Append($"start={m.fStartBearing:F1}, end={m.fEndBearing:F1}");
                    break;

                // ===== TM / 원시 트랙 =====
                //case TM_BEARING_TRACK m:
                //    sb.Append($"TTN={m.unTTN}, RSD={m.eRSDomain}, BL={m.eBLDisplay}, ")
                //      .Append($"brg={m.stCommon.fBearing:F1}°, el={m.stCommon.fElevation:F1}°, ")
                //      .Append($"ID={m.stCommon.eIdentity}, KA={m.stCommon.eKillAssessment}, TQ={m.stCommon.unTQ}, ")
                //      .Append($"tag='{SafeStr(m.stCommon.chTagName)}'");
                //    break;

                case CMS_PRIMITIVE_BEARING_TRACK_INFO m:
                    sb.Append($"src={m.stMember.eSrcID}, TTN={m.stMember.unPTN}, status={m.eTrackStatus}, cat={m.eCategory}, id={m.eIdentity}, ")
                      .Append($"brg={m.stPOS.fBearing:F1}, el={m.stPOS.fElevationAngle:F1}, RSD={m.eRSDomain}");
                    break;

                // ===== IRST(VNG) =====
                case IRST_SYSTEM_STATUS m:
                    sb.Append($"sender={m.stMsgHeader.eSendEquipID}, conn={m.eConnectionStatus}, sw={m.eSwStatus}, ")
                      .Append($"stbAck={m.stStabilizationMode.eAcknowledge}, camModeAck={m.stIrCameraMode.eAcknowledge}, ")
                      .Append($"FOVAck={m.stIrCameraFovStatus.eAcknowledge}, EnvAck={m.stEnvironmentStatus.eAcknowledge}");
                    break;

                case IRST_TRACK_INFO m:
                    sb.Append($"equip={m.eSensorID}, IRNo={m.unIRTrackNo}, TTN={m.unTTN}, SNR={m.stIRDetail.fSNR:F1}, ")
                      .Append($"AC:{m.stIRDetail.fAircraftProb:F0}% HE:{m.stIRDetail.fHeloProb:F0}% SH:{m.stIRDetail.fShipProb:F0}% UN:{m.stIRDetail.fUnknownProb:F0}%");
                    break;

                case IRST_TRACK_DELETE m:
                    sb.Append($"equip={m.eSensorID}, track={m.ulTrackNo}");
                    break;

                //case IRST_PANORAMIC_VIDEO_ELEMENT m:
                //    sb.Append($"equip={m.eSensorID}, pane={m.ulPaneID}, r={m.ulRowNo}, c={m.ulColumnNo}, ")
                //      .Append($"az[{m.fFirstColumnAzimuth:F1}~{m.fLastColmnAzimuth:F1}], el[{m.fFirstRowElevation:F1}~{m.fLastRowElevation:F1}], ")
                //      .Append($"bytes={m.fpixel?.Length ?? 0}");
                //    break;

                //case IRST_MAGNIFIER_VIDEO_ELEMENT m:
                //    sb.Append($"equip={m.eSensorID}, mag={m.byMagnifierID}({m.eIrCameraFovStatus}), mawe={m.ulMaweID}, r={m.ulRowNo}, c={m.ulColumnNo}, ")
                //      .Append($"az[{m.fFirstColumnAzimuth:F1}~{m.fLastColmnAzimuth:F1}], el[{m.fFirstRowElevation:F1}~{m.fLastRowElevation:F1}], ")
                //      .Append($"bytes={m.fpixel?.Length ?? 0}");
                //    break;

                // ===== EOSS VIGY4 / XLR (요약) =====
                case EOSS_VIGY4_SYSTEM_STATUS m:
                    sb.Append($"mode={m.eMode}, sys={m.eSystemStatus}, MAS={m.eMASState}, LRFKey={m.eLRFKeyState}, conn={m.eConnectionStatus}, sw={m.eSwStatus}, ")
                      .Append($"LOS az={m.stLOSStatus.fLOSazimuth:F1}, el={m.stLOSStatus.fLOStrueelevation:F1}, ")
                      .Append($"TV preset={m.stCameraStatusTV.eFOVpreset}, IR preset={m.stCameraStatusMWIR.eFOVpreset}");
                    break;

                case EOSS_XLR_SYSTEM_STATUS m:
                    sb.Append($"mode={m.eMode}, sys={m.eSystemStatus}, FOVSlaving={m.eFOVSlaving}, usedTrk={m.eUsedTracker}, conn={m.eConnectionStatus}, sw={m.eSwStatus}, ")
                      .Append($"LOS az={m.stLOSStatus.fLOSAzimuth:F1}, el={m.stLOSStatus.fLOSTrueElevation:F1}");
                    break;

                // 대표 명령(발행 시 요약)
                case MFC_IRST_ENVIRONMENT_SETTING m:
                    sb.Append($"sensor={m.eSensorID}, sky={m.eSkyFiltering}, sea={m.eSeaFiltering}, atten={m.eAttenuationMode}/{m.fAttenuationAdjust:F1}, mission={m.eMissionCondition}");
                    break;

                case MFC_IRST_CAMERA_MODE_ORDER m:
                    sb.Append($"sensor={m.eSensorID}, mode={m.eIrCameraMode}");
                    break;

                case MFC_VIGY4_SYSTEM_MODE_COMMAND m:
                    sb.Append($"cmd={m.eSystemCmd}");
                    break;

                case MFC_XLR_SYSTEM_MODE_COMMAND m:
                    sb.Append($"cmd={m.eSystemCmd}");
                    break;

                // ===== AIS / ADS-B / DDU =====
                case AIS_SYSTEM_STATUS m:
                    sb.Append($"conn={m.eConnectionStatus}, sw={m.eSwStatus}");
                    break;

                //case AIS_TRACK_INFO m:
                //    sb.Append($"TTN={m.unTTN}, MMSI={m.ulMMSI}, ")
                //      .Append($"Name='{SafeStr(m.stAISDetail.chName)}', Call='{SafeStr(m.stAISDetail.chCallSign)}', ")
                //      .Append($"Lat={m.stAISDetail.dLat:F6}, Lon={m.stAISDetail.dLong:F6}, COG={m.stAISDetail.fCourse:F1}, SOG={m.stAISDetail.fSpeed:F1}");
                //    break;

                case ADSB_SYSTEM_STATUS m:
                    sb.Append($"conn={m.eConnectionStatus}, sw={m.eSwStatus}");
                    break;

                //case ADSB_TRACK_INFO m:
                //    sb.Append($"TTN={m.unTTN}, FL={m.stADSBDetail.unFlightLevel}, Target='{SafeStr(m.stADSBDetail.chTargetID)}', ")
                //      .Append($"Lat={m.stADSBDetail.dLat:F6}, Lon={m.stADSBDetail.dLong:F6}, HDG={m.stADSBDetail.fHeading:F1}, GS={m.stADSBDetail.fSpeed:F1}");
                //    break;

                case DDU_SYSTEM_STATUS m:
                    sb.Append($"conn={m.eConnectionStatus}, sw={m.eSwStatus}");
                    break;

                // ===== 40mm MARLIN ILOS =====
                //case MARIN40_STATUS m:
                //    sb.Append($"conn={m.eConnectionStatus}, sw={m.eSwStatus}, ")
                //      .Append($"ReadyToFire={m.stSystemStatus.eReadyToFire}, Remote={m.stSystemStatus.eRemoteMode}, Safety={m.stSystemStatus.eSafetyOn}");
                //    break;

                //case MARIN40_FIRE_STATUS m:
                //    sb.Append($"misfire={m.eMisfire}, inProgress={m.eFireInProgress}, ")
                //      .Append($"rate={m.stFireStatus.eFireRate}, burst={m.stFireStatus.byBurstLength}, feederSel={m.stFeederStatus.eSelectedFeeder}");
                //    break;

                //case MARIN40_POSITION_STATUS m:
                //    sb.Append($"train={m.fTraining:F1}°, elev={m.fElevation:F1}°");
                //    break;

                //case MFC_40GUN_SYSTEM_STATUS_CONTROL m:
                //    sb.Append($"getInCharge={m.eGetInCharge}, servo={m.eServoOn}, arm={m.eArm}, BS={m.eBattleShort}, auto={m.eWeaponAutomatic}, fuzeInhibit={m.eFuzeInhibit}");
                //    break;

                //case MFC_40GUN_FIRE_STATUS_CONTROL m:
                //    sb.Append($"single={m.eSingleShot}, feederB={m.eFeederSectorB}, fire={m.eFireOrder}, lowRate={m.eLowRateFire}, burst={m.unBurstLength}");
                //    break;

                //case MFC_40GUN_POSITION_CONTROL m:
                //    sb.Append($"train={m.fTraining:F1}°, elev={m.eElevation:F1}°");
                //    break;

                //case MFC_40GUN_FUZE_PROGRAMMING_CONTROL m:
                //    sb.Append($"fuzeMode={m.eFuzeProgrammer}");
                //    break;

                // ===== default =====
                default:
                    sb.Append(payload?.ToString());
                    break;
            }

            return sb.ToString();
        }

        private static string Format(Enum_DebugStatus level, string msg)
        {
            var sb = new StringBuilder(256);
            sb.Append('[').Append(DateTime.Now.ToString("HH:mm:ss.fff")).Append("] | ")
              .Append('[').Append(level).Append("] | ")
              .Append(msg);
            return sb.ToString();
        }
    }
}
