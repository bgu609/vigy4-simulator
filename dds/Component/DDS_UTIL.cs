namespace Pa5455CmsDds.Component
{
    public static class DDS_UTIL
    {
        public static void inputTopicHeaderTime(ST_MSG_HEADER header, DateTime dateTime)
        {
            header.stSendTime.unYear = (ushort)dateTime.Year;
            header.stSendTime.byMonth = (byte)dateTime.Month;
            header.stSendTime.byDay = (byte)dateTime.Day;
            header.stSendTime.byHour = (byte)dateTime.Hour;
            header.stSendTime.byMin = (byte)dateTime.Minute;
            header.stSendTime.usMSec = (ushort)((dateTime.Second * 1000) + dateTime.Millisecond);
        }

        public static DateTime extractDateTime(ST_MSG_HEADER header)
        {
            return new DateTime(
                header.stSendTime.unYear,
                header.stSendTime.byMonth,
                header.stSendTime.byDay,
                header.stSendTime.byHour,
                header.stSendTime.byMin,
                header.stSendTime.usMSec / 1000,
                header.stSendTime.usMSec % 1000
            );
        }

        public static DateTime extractDateTime(ST_TIME_INFO timeInformation)
        {
            return new DateTime(
                timeInformation.unYear,
                timeInformation.byMonth,
                timeInformation.byDay,
                timeInformation.byHour,
                timeInformation.byMin,
                timeInformation.usMSec / 1000,
                timeInformation.usMSec % 1000
            );
        }
    }
}