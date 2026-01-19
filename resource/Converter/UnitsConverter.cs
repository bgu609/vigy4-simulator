using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pa5455CmsResource.Converter
{
    /// <summary>
    /// 각종 단위/각도 변환 및 보조 유틸리티 모음.
    /// - 정적 메서드 + 확장 메서드 제공
    /// - 각도(rad/deg), 거리, 속도, 온도 변환 등
    /// </summary>
    public static class UnitsConverter
    {
        public const double DegPerRad = 180.0 / Math.PI;
        public const double RadPerDeg = Math.PI / 180.0;

        // ---- 각도 변환 ----
        public static double DegToRad(double deg) => deg * RadPerDeg;
        public static double RadToDeg(double rad) => rad * DegPerRad;
        public static float DegToRad(float deg) => (float)(deg * RadPerDeg);
        public static float RadToDeg(float rad) => (float)(rad * DegPerRad);

        // 확장 메서드 버전 (편의)
        public static double ToRad(this double deg) => DegToRad(deg);
        public static double ToDeg(this double rad) => RadToDeg(rad);
        public static float ToRad(this float deg) => DegToRad(deg);
        public static float ToDeg(this float rad) => RadToDeg(rad);

    }
}
